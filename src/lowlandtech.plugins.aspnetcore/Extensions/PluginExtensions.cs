namespace LowlandTech.Plugins.AspNetCore.Extensions;

public static class PluginExtensions
{
    /// <summary>
    /// Adds plugins to the service registry from the configuration.
    /// </summary>
    /// <param name="services">The service registry</param>
    public static void AddPlugins(this IServiceCollection services)
    {
        // Build service provider to access configuration and logging
        // Note: This creates a temporary provider; the real one is built later by the caller
        var provider = services.BuildServiceProvider();
        var mvcBuilder = services.AddControllers();
        // Try to obtain an ILoggerFactory from the registered services; fall back to a temporary factory if none is registered
        var factory = provider.GetService<ILoggerFactory>();
        
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
        
        var logger = factory.CreateLogger("LowlandTech.Plugins.Extensions");

        // IConfiguration may not be registered in some test scenarios - treat as empty configuration when absent
        var configuration = provider.GetService<IConfiguration>();

        // Use the shared discovery service
        var discoveredPlugins = LowlandTech.Plugins.Services.PluginDiscoveryService.DiscoverPlugins(configuration, logger);
        
        // Register each discovered plugin
        foreach (var plugin in discoveredPlugins)
        {
            try
            {
                services.AddPlugin(plugin);
                mvcBuilder.AddApplicationPart(plugin.GetType().Assembly);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error registering plugin {0}", plugin.Name);
            }
        }
        
        logger.LogInformation("Plugin discovery completed. {Count} plugins registered.", 
            services.Count(s => s.ServiceType == typeof(IPlugin)));
    }

    /// <summary>
    /// Adds a plugin to the service registry.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="plugin"></param>
    public static void AddPlugin(this IServiceCollection services, IPlugin plugin)
    {
        // Validate arguments up-front to provide clear exceptions for callers
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (plugin is null) throw new ArgumentNullException(nameof(plugin));

        var pluginId = Guard.Against.MissingPluginId(plugin, nameof(plugin), $"ExtensionPlugin '{plugin.GetType().Name}' does not provide a plugin id.");

        // add plugin to the installed plugins list - check if plugin is already registered by type
        if (services.Any(s => s.ImplementationType == plugin.GetType())) return;

        // Check for duplicate plugin IDs among already-registered IPlugin instances/descriptors
        var existingPluginDescriptors = services.Where(s => s.ServiceType == typeof(IPlugin)).ToList();
        foreach (var desc in existingPluginDescriptors)
        {
            // If there's an instance registered, inspect its type attribute
            if (desc.ImplementationInstance is IPlugin existingInstance)
            {
                if (Attribute.GetCustomAttribute(existingInstance.GetType(), typeof(PluginId)) is PluginId existingIdAttr && existingIdAttr.Id == pluginId)
                {
                    throw new ArgumentException($"Duplicate plugin id '{pluginId}' detected.", nameof(plugin));
                }
            }

            // If there is an implementation type registered, inspect its attribute too
            if (desc.ImplementationType is not null)
            {
                if (Attribute.GetCustomAttribute(desc.ImplementationType, typeof(PluginId)) is PluginId existingTypeIdAttr && existingTypeIdAttr.Id == pluginId)
                {
                    throw new ArgumentException($"Duplicate plugin id '{pluginId}' detected.", nameof(plugin));
                }
            }
        }

        // Assign a default name if none provided
        if (string.IsNullOrWhiteSpace(plugin.Name))
        {
            // Use the plugin type name as a sensible default
            plugin.Name = plugin.GetType().Name;
        }

        // install plugin
        plugin.Install(services);
        services.AddSingleton<IPlugin>(plugin);
    }

    /// <summary>
    /// Adds a plugin to the service registry.  
    /// </summary>
    /// <typeparam name="T">The plugin</typeparam>
    /// <param name="services">The service registry</param>
    public static void AddPlugin<T>(this IServiceCollection services) where T : IPlugin, new()
    {
        var plugin = new T();
        plugin.Name = plugin.GetType().Namespace!;
        services.AddPlugin(plugin);
    }

    /// <summary>
    /// Use plugins.
    /// </summary>
    /// <param name="app">The ioc container</param>
    /// <param name="host">The app host</param>
    public static void UsePlugins(this WebApplication app, object? host = null)
    {
        var serviceProvider = app.Services;
        var scope = serviceProvider.CreateScope();
        var plugins = scope.ServiceProvider.GetServices<IPlugin>();
        // and configure plugins;
        foreach (var plugin in plugins)
        {
            plugin.Configure(app.Services, host ?? app);
        }
    }
}
