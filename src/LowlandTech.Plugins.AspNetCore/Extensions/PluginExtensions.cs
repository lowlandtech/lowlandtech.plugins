namespace LowlandTech.Plugins.AspNetCore.Extensions;

public static class PluginExtensions
{
    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t is not null)!;
        }
    }

    /// <summary>
    /// Adds plugins to the service registry from the configuration.
    /// </summary>
    /// <param name="services">The service registry</param>
    public static void AddPlugins(this IServiceCollection services)
    {
        // Build service provider to access configuration and logging
        // Note: This creates a temporary provider; the real one is built later by the caller
        var provider = services.BuildServiceProvider();

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

        List<PluginConfig> pluginList = new List<PluginConfig>();

        if (configuration is not null)
        {
            logger.LogDebug("Configuration is available, attempting to parse plugin configuration");
            
            // Primary attempt: bind to section "Plugins" => [ { Name, IsActive }, ... ]
            var section = configuration.GetSection(PluginOptions.Name);
            var primaryList = section.Get<List<PluginConfig>>() ?? new List<PluginConfig>();
            logger.LogDebug("Primary binding attempt: {Count} plugins", primaryList.Count);
            
            // Log what was bound
            foreach (var p in primaryList)
            {
                logger.LogDebug("  Primary bound: Name='{Name}', IsActive={IsActive}", p.Name ?? "(null)", p.IsActive);
            }
            
            // Filter out any plugins with null/empty names from bad binding
            pluginList = primaryList.Where(p => !string.IsNullOrWhiteSpace(p.Name)).ToList();
            logger.LogDebug("After filtering empty names: {Count} plugins remain", pluginList.Count);

            // Secondary attempt: some tests use nested "Plugins:Plugins" keys. Try nested section.
            if (!pluginList.Any())
            {
                logger.LogDebug("Attempting nested section binding...");
                var nested = section.GetSection(PluginOptions.Name);
                var nestedList = nested.Get<List<PluginConfig>>() ?? new List<PluginConfig>();
                logger.LogDebug("Nested section binding attempt: {Count} plugins", nestedList.Count);
                
                foreach (var p in nestedList)
                {
                    logger.LogDebug("  Nested bound: Name='{Name}', IsActive={IsActive}", p.Name ?? "(null)", p.IsActive);
                }
                
                pluginList = nestedList.Where(p => !string.IsNullOrWhiteSpace(p.Name)).ToList();
                logger.LogDebug("After filtering empty names: {Count} plugins remain", pluginList.Count);
            }

            // Tertiary attempt: scan configuration keys for entries like "Plugins:Plugins:0:Name" or "Plugins:0:Name"
            if (!pluginList.Any())
            {
                logger.LogDebug("No valid plugins after binding attempts, starting manual key scanning...");
                
                var all = configuration.AsEnumerable().Where(kv => !string.IsNullOrWhiteSpace(kv.Value)).ToList();
                
                logger.LogDebug("Scanning {Count} configuration entries for plugin definitions", all.Count);
                
                // Log all config keys starting with "Plugins:"
                var pluginKeys = all.Where(kv => kv.Key.StartsWith(PluginOptions.Name + ":", StringComparison.OrdinalIgnoreCase)).ToList();
                logger.LogDebug("Found {Count} keys starting with 'Plugins:'", pluginKeys.Count);
                
                // Group entries by looking for Name and IsActive pairs
                var grouped = new Dictionary<string, PluginConfig>();
                
                foreach (var kv in pluginKeys)
                {
                    logger.LogDebug("Processing config key: '{Key}' = '{Value}'", kv.Key, kv.Value);
                    
                    // Extract the index/key pattern - handle both "Plugins:0:Name" and "Plugins:Plugins:0:Name"
                    var remainder = kv.Key.Substring((PluginOptions.Name + ":").Length);
                    var parts = remainder.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    
                    logger.LogDebug("  Remainder: '{Remainder}', Parts: [{Parts}]", remainder, string.Join(", ", parts));
                    
                    if (parts.Length < 2)
                    {
                        logger.LogDebug("  Skipping - not enough parts");
                        continue;
                    }
                    
                    // Find the numeric index - could be first part or second part depending on nesting
                    string? indexKey = null;
                    string? fieldName = null;
                    
                    // Check if first part is "Plugins" (nested structure)
                    if (parts[0].Equals(PluginOptions.Name, StringComparison.OrdinalIgnoreCase) && parts.Length >= 3)
                    {
                        // "Plugins:Plugins:0:Name" format
                        indexKey = parts[1]; // the index after the repeated "Plugins"
                        fieldName = parts[2]; // the field name
                        logger.LogDebug("  Detected nested format - Index: '{Index}', Field: '{Field}'", indexKey, fieldName);
                    }
                    else if (parts.Length == 2)
                    {
                        // "Plugins:0:Name" format
                        indexKey = parts[0];
                        fieldName = parts[1];
                        logger.LogDebug("  Detected flat format - Index: '{Index}', Field: '{Field}'", indexKey, fieldName);
                    }
                    else if (parts.Length >= 3)
                    {
                        // Fallback: use second-to-last as index and last as field
                        indexKey = parts[^2];
                        fieldName = parts[^1];
                        logger.LogDebug("  Using fallback format - Index: '{Index}', Field: '{Field}'", indexKey, fieldName);
                    }
                    
                    if (indexKey is null || fieldName is null)
                    {
                        logger.LogDebug("  Skipping - could not extract index or field");
                        continue;
                    }
                    
                    if (!grouped.ContainsKey(indexKey))
                    {
                        grouped[indexKey] = new PluginConfig();
                        logger.LogDebug("  Created new PluginConfig for index '{Index}'", indexKey);
                    }
                    
                    if (fieldName.Equals("Name", StringComparison.OrdinalIgnoreCase))
                    {
                        grouped[indexKey].Name = kv.Value ?? string.Empty;
                        logger.LogDebug("  Set Name = '{Name}'", kv.Value);
                    }
                    else if (fieldName.Equals("IsActive", StringComparison.OrdinalIgnoreCase))
                    {
                        grouped[indexKey].IsActive = bool.TryParse(kv.Value, out var bVal) ? bVal : false;
                        logger.LogDebug("  Set IsActive = {IsActive}", grouped[indexKey].IsActive);
                    }
                }
                
                pluginList = grouped.Values.Where(p => !string.IsNullOrWhiteSpace(p.Name)).ToList();
                logger.LogDebug("Manual parsing completed: {Count} plugins extracted", pluginList.Count);
            }
            else
            {
                logger.LogDebug("Skipping manual parsing - already have {Count} valid plugins from binding", pluginList.Count);
            }
        }
        else
        {
            logger.LogDebug("Configuration is null - no plugins to discover");
        }

        logger.LogInformation("Starting plugin discovery with {Count} configured plugins", pluginList.Count);
        foreach (var cfg in pluginList)
        {
            logger.LogInformation("  - {Name} (IsActive: {IsActive})", cfg.Name ?? "(null)", cfg.IsActive);
        }

        try
        {
            // Get the currently executing assembly
            var currentAssembly = Assembly.GetExecutingAssembly();

            // Get the AssemblyLoadContext for the current assembly
            var loadContext = AssemblyLoadContext.GetLoadContext(currentAssembly)!;

            // Prepare candidate roots to look for plugin assemblies
            var candidateRoots = new List<string?>
            {
                new FileInfo(currentAssembly.Location).DirectoryName,
                AppContext.BaseDirectory,
                Directory.GetCurrentDirectory()
            };

            candidateRoots = candidateRoots.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct().ToList();

            // track whether any plugins were added
            var addedAny = false;

            // Attempt to load the assembly for configured plugins
            foreach (var config in pluginList.Where(p => p.IsActive == true))
            {
                try
                {
                    logger.LogDebug("Attempting to load plugin: {Name}", config.Name);
                    
                    Assembly? assembly = null;
                    Type? pluginType = null;

                    // First, try to find the assembly already loaded in the current AppDomain
                    assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => string.Equals(a.GetName().Name, config.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(a.GetName().Name, config.Name.Replace('.', '-'), StringComparison.OrdinalIgnoreCase));

                    if (assembly is not null)
                    {
                        logger.LogDebug("Found assembly {AssemblyName} already loaded", assembly.GetName().Name);
                    }

                    // If not found, try to load by assembly name from the default load context
                    if (assembly is null)
                    {
                        try
                        {
                            assembly = Assembly.Load(new AssemblyName(config.Name));
                            logger.LogDebug("Loaded assembly {AssemblyName} via Assembly.Load", assembly.GetName().Name);
                        }
                        catch (Exception ex)
                        {
                            logger.LogDebug("Could not load assembly via Assembly.Load: {Message}", ex.Message);
                            // ignore and fallback to file-based loading
                        }
                    }

                    // If still not found, search for the file in candidate roots
                    string? assemblyPath = null;
                    if (assembly is null)
                    {
                        foreach (var root in candidateRoots)
                        {
                            var candidate = Path.Combine(root!, config.Name + ".dll");
                            if (File.Exists(candidate))
                            {
                                assemblyPath = candidate;
                                logger.LogDebug("Found assembly file at {Path}", assemblyPath);
                                break;
                            }
                        }

                        // If no file found, as a last resort search all DLLs in candidate roots for a matching type/namespace
                        if (assemblyPath is null)
                        {
                            foreach (var root in candidateRoots)
                            {
                                try
                                {
                                    foreach (var file in Directory.EnumerateFiles(root!, "*.dll", SearchOption.TopDirectoryOnly))
                                    {
                                        try
                                        {
                                            var name = Path.GetFileNameWithoutExtension(file);
                                            if (string.Equals(name, config.Name, StringComparison.OrdinalIgnoreCase))
                                            {
                                                assemblyPath = file;
                                                break;
                                            }

                                            // quick open to inspect types may be expensive, so only attempt if name contains segments
                                            if (name.Contains('.') && name.EndsWith(config.Name.Split('.').Last()))
                                            {
                                                assemblyPath = file;
                                                break;
                                            }
                                        }
                                        catch { }
                                    }
                                }
                                catch { }

                                if (assemblyPath is not null) break;
                            }
                        }

                        if (assemblyPath is not null)
                        {
                            var assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
                            if (assemblyName is not null)
                            {
                                assembly = loadContext.LoadFromAssemblyName(assemblyName);
                                logger.LogDebug("Loaded assembly from file {Path}", assemblyPath);
                            }
                        }
                    }

                    // If assembly still null, try to find a suitable Type in already-loaded assemblies
                    if (assembly is not null)
                    {
                        pluginType = GetLoadableTypes(assembly).FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);
                        if (pluginType is not null)
                        {
                            logger.LogDebug("Found IPlugin type {TypeName} in assembly {AssemblyName}", pluginType.FullName, assembly.GetName().Name);
                        }
                    }
                    else
                    {
                        logger.LogDebug("Assembly not found, searching loaded assemblies for matching namespace");
                        // search loaded assemblies for a type whose namespace or assembly name matches config.Name
                        foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            try
                            {
                                var candidateType = GetLoadableTypes(a).FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface && (t.Namespace == config.Name || (t.Namespace is not null && t.Namespace.StartsWith(config.Name, StringComparison.OrdinalIgnoreCase)) || a.GetName().Name == config.Name));
                                if (candidateType is not null)
                                {
                                    pluginType = candidateType;
                                    assembly = a;
                                    logger.LogDebug("Found IPlugin type {TypeName} in assembly {AssemblyName} via namespace search", pluginType.FullName, assembly.GetName().Name);
                                    break;
                                }
                            }
                            catch { }
                        }
                    }

                    if (pluginType is null)
                    {
                        logger.LogWarning("No type implementing IPlugin found for plugin {0}", config.Name);
                        continue;
                    }

                    // Create an instance of the type
                    var plugin = (IPlugin)Activator.CreateInstance(pluginType)!;

                    // record assembly for plugin (useful for assembly load context checks)
                    if (plugin is LowlandTech.Plugins.Types.Plugin p)
                    {
                        if (assembly is not null)
                        {
                            p.Assemblies.Add(assembly);
                        }
                    }

                    // Use the plugin
                    plugin.Name = config.Name;
                    plugin.IsActive = config.IsActive;
                    services.AddPlugin(plugin);
                    addedAny = true;

                    logger.LogInformation("{0}: Found and instantiated", pluginType.FullName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error loading plugin {0}", config.Name);
                }
            }

            // If no plugins were added via discovery, attempt a best-effort fallback: load DLLs from candidate roots and search for types
            if (!addedAny && pluginList.Any())
            {
                logger.LogDebug("No plugins added via primary discovery, attempting fallback DLL scan");
                foreach (var root in candidateRoots)
                {
                    try
                    {
                        foreach (var file in Directory.EnumerateFiles(root!, "*.dll", SearchOption.TopDirectoryOnly))
                        {
                            try
                            {
                                // skip already loaded assemblies
                                var loaded = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => string.Equals(a.Location, file, StringComparison.OrdinalIgnoreCase));
                                Assembly asm = loaded ?? loadContext.LoadFromAssemblyPath(file);

                                foreach (var config in pluginList.Where(p => p.IsActive == true))
                                {
                                    try
                                    {
                                        var candidateType = GetLoadableTypes(asm).FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface && (t.Namespace == config.Name || (t.Namespace is not null && t.Namespace.StartsWith(config.Name, StringComparison.OrdinalIgnoreCase)) || t.FullName?.StartsWith(config.Name) == true || t.Name == config.Name.Split('.').Last()));
                                        if (candidateType is not null)
                                        {
                                            var plugin = (IPlugin)Activator.CreateInstance(candidateType)!;
                                            plugin.Name = config.Name;
                                            plugin.IsActive = config.IsActive;
                                            services.AddPlugin(plugin);
                                            logger.LogInformation("Loaded plugin {0} from file {1}", candidateType.FullName, file);
                                        }
                                    }
                                    catch { }
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }
            
            logger.LogInformation("Plugin discovery completed. {Count} plugins registered.", services.Count(s => s.ServiceType == typeof(IPlugin)));
        }
        catch (Exception ex)
        {
            // Handle the exception if the assembly cannot be loaded
            var logger2 = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("LowlandTech.Plugins.Extensions");
            logger2.LogError("Error loading assembly: {0}", ex.Message);
        }
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
