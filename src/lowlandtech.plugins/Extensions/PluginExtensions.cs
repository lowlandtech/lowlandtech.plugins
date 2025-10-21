namespace LowlandTech.Plugins.Extensions;

/// <summary>
/// Extensions for plugins.
/// </summary>
public static class PluginExtensions
{
    /// <summary>
    /// Adds a plugin to the service registry.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="plugin"></param>
    public static void AddPlugin(this ServiceRegistry services, IPlugin plugin)
    {
        var pluginId = Guard.Against.MissingPluginId(plugin, nameof(plugin), $"ExtensionPlugin '{plugin.GetType().Name}' does not provide a plugin id.");

        // add plugin to the installed plugins list
        // Check if the plugin type is already registered as IPlugin
        if (services.Any(s => s.ServiceType == typeof(IPlugin) && s.ImplementationType == plugin.GetType())) return;
        
        // install plugin - cast to Plugin to ensure ServiceRegistry overload is called
        if (plugin is Plugin p)
        {
            p.Install(services);

            // Track the assembly where the plugin type came from for later reflection or diagnostics
            try
            {
                var asm = plugin.GetType().Assembly;
                if (asm is not null && !p.Assemblies.Contains(asm)) p.Assemblies.Add(asm);
            }
            catch { }
        }
        else
        {
            plugin.Install(services);
        }
        
        services.For<IPlugin>().Add(plugin).Named(pluginId);
    }

    /// <summary>
    /// Adds plugins to the service registry from the configuration.
    /// </summary>
    /// <param name="services">The service registry</param>
    public static void AddPlugins(this ServiceRegistry services)
    {
        ILoggerFactory? factory = null;
        IConfiguration? configuration = null;
        ILogger logger;

        try
        {
            // Use Lamar container to resolve any registrations that were added to the ServiceRegistry
            using var container = new Container(services);
            factory = container.TryGetInstance<ILoggerFactory>();
            configuration = container.TryGetInstance<IConfiguration>();

            // If no logger factory registered, create a console logger with debug level for troubleshooting
            if (factory is null)
            {
                factory = LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Debug);
                    builder.AddFilter("LowlandTech.Plugins", LogLevel.Debug);
                });
            }

            logger = factory.CreateLogger("LowlandTech.Plugins.Extensions");
        }
        catch
        {
            // If building a Lamar container fails for any reason, fall back to a temporary logger factory
            factory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
                builder.AddFilter("LowlandTech.Plugins", LogLevel.Debug);
            });

            logger = factory.CreateLogger("LowlandTech.Plugins.Extensions");
        }

        // Use the shared discovery service
        var discoveredPlugins = Services.PluginDiscoveryService.DiscoverPlugins(configuration, logger);

        // Register each discovered plugin
        foreach (var plugin in discoveredPlugins)
        {
            try
            {
                services.AddPlugin(plugin);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error registering plugin {0}", plugin.Name);
            }
        }

        logger.LogInformation("Plugin registration completed. {Count} plugins registered in ServiceRegistry.",
            services.Count(s => s.ServiceType == typeof(IPlugin)));
    }


    /// <summary>
    /// Use plugins.
    /// </summary>
    /// <param name="container">The ioc container</param>
    /// <param name="host">The app host</param>
    public static async Task UsePlugins(this IContainer container, object? host = null)
    {
        var plugins = container.GetAllInstances<IPlugin>();
        // and configure plugins;
        foreach (var plugin in plugins)
        {
            // Cast to Plugin to ensure IContainer overload is called
            if (plugin is Plugin p)
            {
                await p.Configure(container, host);
            }
            else
            {
                await plugin.Configure(container, host);
            }
        }
    }

    /// <summary>
    /// Registers and configures a plugin of type <typeparamref name="T"/> within the specified service registry.
    /// </summary>
    /// <remarks>This method creates an instance of the plugin type <typeparamref name="T"/>, installs it into
    /// the service registry, and configures it using the built service container. The plugin's <see
    /// cref="IPlugin.Install"/> and <see cref="IPlugin.Configure"/> methods are invoked during this process.</remarks>
    /// <typeparam name="T">The type of the plugin to register. Must implement <see cref="IPlugin"/> and have a parameterless constructor.</typeparam>
    /// <param name="services">The service registry to which the plugin will be added.</param>
    /// <param name="host">An optional host object that can be used during plugin configuration. Can be <see langword="null"/>.</param>
    public static void UsePlugin<T>(this ServiceRegistry services, object? host = null)
        where T : class, IPlugin, new()
    {
        // Create an instance of the type
        var plugin = new T();
        // and configure plugins;
        plugin.Install(services);
        //var container = services.BuildServiceProvider().GetRequiredService<IContainer>();
        //plugin.Configure(container, host);
    }
}