namespace LowlandTech.Plugins.Tests.Fakes;

/// <summary>
/// Provides a factory for creating and configuring a test instance of a web application.
/// </summary>
/// <remarks>
/// This factory is designed to support integration testing scenarios by bootstrapping a real Kestrel
/// server with a predictable development environment. It allows for serving static web assets, configuring dependency
/// injection overrides, and ensuring an ephemeral IPv4 port is used for the test server.
/// </remarks>
/// <typeparam name="TProgram">
/// The entry point class of the application under test. This is typically the class containing the application's
/// <c>Main</c> method.
/// </typeparam>
public sealed class AppFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class, new()
{
    public AppFactory(SampleContext? db = null)
    {
        // Reduce file watcher pressure for CI/test environments
        Environment.SetEnvironmentVariable("DOTNET_USE_POLLING_FILE_WATCHER", "1");
        Environment.SetEnvironmentVariable("DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE", "false");

        // Pick workspace root per platform, then append the project subfolder
        var workspaceRoot = ResolveWorkspaceRoot();

        _leaf = typeof(TProgram).Name.Contains("Api", StringComparison.OrdinalIgnoreCase) ? "api" : "app";
        _contentRoot = Path.Combine(workspaceRoot, _leaf);

        Db = db ?? new SampleContext();
        Db.Database.EnsureCreated();
    }

    private static string ResolveWorkspaceRoot()
    {
        var current = AppContext.BaseDirectory;

        while (!string.IsNullOrEmpty(current))
        {
            var candidate = new DirectoryInfo(current);
            var srcFolder = Path.Combine(candidate.FullName, "src");
            var solution = Directory.EnumerateFiles(candidate.FullName, "*.sln", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();

            if (Directory.Exists(srcFolder) && solution is not null)
            {
                return candidate.FullName;
            }

            current = candidate.Parent?.FullName;
        }

        return OperatingSystem.IsWindows()
            ? $"C:\\Workspaces\\lowlandtech.accounts\\src"
            : "/root/repo/src";
    }

    private IHost? _host;
    private string _leaf;
    private string _contentRoot;
    public SampleContext Db { get; set; }

    // Optional hook for additional test-time DI registrations
    private Action<IServiceCollection>? _extraServices;
    public void ConfigureTestServices(Action<IServiceCollection> configure)
        => _extraServices = configure;

    public Uri ServerAddress
    {
        get
        {
            EnsureServer();
            return new Uri(ClientOptions.BaseAddress.ToString());
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            // Serve the actual app files for static web assets etc.
            .UseContentRoot(_contentRoot)
            .UseWebRoot("wwwroot")
            .UseStaticWebAssets()

            // Ensure Kestrel + ephemeral IPv4 port (not the TestServer)
            .UseKestrel()
            .PreferHostingUrls(true) // Make UseUrls win over other configuration
            .UseUrls("http://127.0.0.1:0")

            // Keep test-time environment predictable
            .UseEnvironment("Development");

        // DI overrides hook
        builder.ConfigureServices(services =>
        {
            if (_leaf == "api")
            {
                // Replace the DbContext registration to use our in-memory instance
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDbContextFactory<SampleContext>));
                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }

                services.AddScoped<IDbContextFactory<SampleContext>>(_ => new DbContextFactory<SampleContext>(Db));
            }
        });

        if (_extraServices is not null)
        {
            builder.ConfigureServices(_extraServices);
        }
    }

    private void EnsureServer()
    {
        if (_host is null)
        {
            // Force WAF to bootstrap via our CreateHost override
            using var _ = CreateDefaultClient();
        }
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // 1) Build the TestServer host that WAF expects
        var testHost = builder.UseEnvironment("Development").Build();
        testHost.Start();

        // 2) Build a *real* Kestrel host on an ephemeral IPv4 port
        builder.ConfigureWebHost(webHost =>
        {
            webHost
                .UseKestrel(options =>
                {
                    // Bind explicitly to IPv4 loopback on a dynamic port
                    options.Listen(IPAddress.Loopback, 0);
                })
                .PreferHostingUrls(true) // ensure our urls/listen wins
                .UseUrls("http://127.0.0.1:0")
                .UseEnvironment("Development");
        });

        _host = builder.Build();
        _host.Start();

        // 3) Resolve an actual bound address (robust wait + fallback)
        var server = _host.Services.GetRequiredService<IServer>();
        var feature = server.Features.Get<IServerAddressesFeature>();

        Uri? bound = null;
        var deadline = DateTime.UtcNow.AddSeconds(5);
        while (DateTime.UtcNow < deadline && bound is null)
        {
            var candidate = feature?.Addresses?
                .Select(a => new Uri(a))
                .FirstOrDefault(u => u.IsAbsoluteUri && u.Port != 0);

            if (candidate is not null)
            {
                bound = candidate;
                break;
            }

            // Small backoff to avoid busy-waiting
            Thread.Sleep(50);
        }

        if (bound is null)
        {
            // Fallback: try an HTTP ping to wake the pipeline, then re-check
            try
            {
                using var http = new HttpClient { BaseAddress = new Uri("http://127.0.0.1/") };
                // A no-op poke; may fail harmlessly if port isn't determined
                _ = http.GetAsync("/");
            }
            catch { /* ignore */ }

            Thread.Sleep(100);

            bound = feature?.Addresses?
                .Select(a => new Uri(a))
                .FirstOrDefault(u => u.IsAbsoluteUri && u.Port != 0);
        }

        if (bound is null)
            throw new InvalidOperationException("Kestrel did not publish a bound address within the timeout.");

        // 4) Publish the concrete root to ClientOptions for downstream tests
        var root = new Uri(bound.GetLeftPart(UriPartial.Authority) + "/");
        ClientOptions.BaseAddress = root;

        return testHost;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing) return;

        try { _host?.Dispose(); }
        catch { /* swallow for test stability */ }
        _host = null;
    }
}
