namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC01_AspNetCore;

/// <summary>
/// Test plugin for ASP.NET Core integration tests.
/// </summary>
[PluginId("550e8400-e29b-41d4-a716-446655440000")]
public class TestPlugin : Plugin
{
    public bool InstallCalled { get; private set; }
    public bool ConfigureCalled { get; private set; }
    public IServiceProvider? ConfigureServiceProvider { get; private set; }
    public object? ConfigureHost { get; private set; }

    public override void Install(IServiceCollection services)
    {
        InstallCalled = true;
        // Register a test service
        services.AddSingleton<TestService>();
    }

    public override Task Configure(IServiceProvider container, object? host = null)
    {
        ConfigureCalled = true;
        ConfigureServiceProvider = container;
        ConfigureHost = host;

        if (host is WebApplication app)
        {
            // Register a test endpoint
            app.MapGet("/test-plugin", () => new { message = "Test plugin endpoint", pluginName = Name });
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// Test service registered by the test plugin.
/// </summary>
public class TestService
{
    public string GetMessage() => "Test service from plugin";
}
