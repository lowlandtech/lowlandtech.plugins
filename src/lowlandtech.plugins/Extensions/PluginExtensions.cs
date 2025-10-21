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
        if (services.All(s => s.ServiceType == plugin.GetType())) return;
        // install plugin
        plugin.Install(services);
        services.For<IPlugin>().Add(plugin).Named(pluginId);
    }

    /// <summary>
    /// Adds plugins to the service registry from the configuration.
    /// </summary>
    /// <param name="services">The service registry</param>
    public static void AddPlugins(this ServiceRegistry services)
    {
        var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<ILoggerFactory>();
        var logger = factory.CreateLogger("LowlandTech.Plugins.Extensions");
        var configuration = provider.GetRequiredService<IConfiguration>();

        var options = new PluginOptions
        {
            Plugins = configuration
                .GetSection(PluginOptions.Name)
                .Get<List<PluginConfig>>()!
        };

        try
        {
            // Get the currently executing assembly
            var currentAssembly = Assembly.GetExecutingAssembly();

            // Get the AssemblyLoadContext for the current assembly
            var loadContext = AssemblyLoadContext.GetLoadContext(currentAssembly)!;
            var root = Path.Combine(new FileInfo(currentAssembly.Location).DirectoryName!);

            // Attempt to load the assembly
            foreach (var config in options.Plugins.Where(p => p.IsActive == true))
            {

                var assemblyName = AssemblyName.GetAssemblyName(Path.Combine(root, config.Name + ".dll"));

                if (assemblyName is null) continue;

                var assembly = loadContext.LoadFromAssemblyName(assemblyName);

                // Do something with the loaded assembly
                logger.LogInformation("Assembly {0} loaded successfully.", assembly.FullName);

                // Get the first type that implements the IPlugin interface
                var pluginType = assembly.GetTypes().FirstOrDefault(type => typeof(IPlugin).IsAssignableFrom(type));

                if (pluginType is not null)
                {
                    // Create an instance of the type
                    var plugin = (IPlugin)Activator.CreateInstance(pluginType)!;

                    // Use the plugin
                    plugin.Name = config.Name;
                    plugin.IsActive = config.IsActive;
                    services.AddPlugin(plugin);

                    logger.LogInformation("{0}: Found and instantiated", pluginType.FullName);
                }
                else
                {
                    logger.LogWarning("No type implementing IPlugin found in the assembly.");
                }
            }
        }
        catch (Exception ex)
        {
            // Handle the exception if the assembly cannot be loaded
            logger.LogError("Error loading assembly: {0}", ex.Message);
        }
    }


    /// <summary>
    /// Use plugins.
    /// </summary>
    /// <param name="container">The ioc container</param>
    /// <param name="host">The app host</param>
    public static void UsePlugins(this IContainer container, object? host = null)
    {
        var plugins = container.GetAllInstances<IPlugin>();
        // and configure plugins;
        foreach (var plugin in plugins)
        {
            plugin.Configure(container, host);
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