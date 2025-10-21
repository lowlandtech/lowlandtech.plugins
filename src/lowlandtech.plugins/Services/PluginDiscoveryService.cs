namespace LowlandTech.Plugins.Services;

/// <summary>
///     Internal service for discovering and loading plugins from configuration.
///     Shared between different IoC container implementations.
/// </summary>
public static class PluginDiscoveryService
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
    ///     Discovers and loads plugins from configuration.
    /// </summary>
    /// <param name="configuration">The configuration to read plugin settings from. Can be null.</param>
    /// <param name="logger">The logger for diagnostics.</param>
    /// <returns>A list of instantiated plugin instances ready to be registered.</returns>
    public static List<IPlugin> DiscoverPlugins(IConfiguration? configuration, ILogger logger)
    {
        var discoveredPlugins = new List<IPlugin>();
        var pluginList = ParsePluginConfiguration(configuration, logger);

        logger.LogInformation("Starting plugin discovery with {Count} configured plugins", pluginList.Count);
        foreach (var cfg in pluginList)
            logger.LogInformation("  - {Name} (IsActive: {IsActive})", cfg.Name ?? "(null)", cfg.IsActive);

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
            foreach (var config in pluginList.Where(p => p.IsActive))
                try
                {
                    logger.LogDebug("Attempting to load plugin: {Name}", config.Name);

                    Assembly? assembly = null;
                    Type? pluginType = null;

                    // First, try to find the assembly already loaded in the current AppDomain
                    assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a =>
                        string.Equals(a.GetName().Name, config.Name, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(a.GetName().Name, config.Name.Replace('.', '-'),
                            StringComparison.OrdinalIgnoreCase));

                    if (assembly is not null)
                        logger.LogDebug("Found assembly {AssemblyName} already loaded", assembly.GetName().Name);

                    // If not found, try to load by assembly name from the default load context
                    if (assembly is null)
                        try
                        {
                            assembly = Assembly.Load(new AssemblyName(config.Name));
                            logger.LogDebug("Loaded assembly {AssemblyName} via Assembly.Load",
                                assembly.GetName().Name);
                        }
                        catch (Exception ex)
                        {
                            logger.LogDebug("Could not load assembly via Assembly.Load: {Message}", ex.Message);
                            // ignore and fallback to file-based loading
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
                            foreach (var root in candidateRoots)
                            {
                                try
                                {
                                    foreach (var file in Directory.EnumerateFiles(root!, "*.dll",
                                                 SearchOption.TopDirectoryOnly))
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
                                        catch
                                        {
                                        }
                                }
                                catch
                                {
                                }

                                if (assemblyPath is not null) break;
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
                        pluginType = GetLoadableTypes(assembly).FirstOrDefault(t =>
                            typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);
                        if (pluginType is not null)
                            logger.LogDebug("Found IPlugin type {TypeName} in assembly {AssemblyName}",
                                pluginType.FullName, assembly.GetName().Name);
                    }
                    else
                    {
                        logger.LogDebug("Assembly not found, searching loaded assemblies for matching namespace");
                        // search loaded assemblies for a type whose namespace or assembly name matches config.Name
                        foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                            try
                            {
                                var candidateType = GetLoadableTypes(a).FirstOrDefault(t =>
                                    typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface &&
                                    (t.Namespace == config.Name ||
                                     (t.Namespace is not null && t.Namespace.StartsWith(config.Name,
                                         StringComparison.OrdinalIgnoreCase)) ||
                                     a.GetName().Name == config.Name));
                                if (candidateType is not null)
                                {
                                    pluginType = candidateType;
                                    assembly = a;
                                    logger.LogDebug(
                                        "Found IPlugin type {TypeName} in assembly {AssemblyName} via namespace search",
                                        pluginType.FullName, assembly.GetName().Name);
                                    break;
                                }
                            }
                            catch
                            {
                            }

                        // Additional broad search: match on type full name, namespace or assembly name containing config.Name
                        if (pluginType is null)
                        {
                            logger.LogDebug("Performing broad scan of loaded assemblies for types containing '{Name}'", config.Name);
                            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                            {
                                try
                                {
                                    var candidateType = GetLoadableTypes(a).FirstOrDefault(t =>
                                        typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface &&
                                        (
                                            (t.FullName is not null && t.FullName.IndexOf(config.Name, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                            (t.Namespace is not null && t.Namespace.IndexOf(config.Name, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                            (a.GetName().Name is not null && a.GetName().Name.IndexOf(config.Name, StringComparison.OrdinalIgnoreCase) >= 0)
                                        ));
                                    if (candidateType is not null)
                                    {
                                        pluginType = candidateType;
                                        assembly = a;
                                        logger.LogDebug("Found IPlugin type {TypeName} in assembly {AssemblyName} via broad scan",
                                            pluginType.FullName, assembly.GetName().Name);
                                        break;
                                    }
                                }
                                catch { }
                            }
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
                    if (plugin is Plugin p)
                        if (assembly is not null)
                            p.Assemblies.Add(assembly);

                    // Use the plugin
                    plugin.Name = config.Name;
                    plugin.IsActive = config.IsActive;

                    discoveredPlugins.Add(plugin);
                    addedAny = true;

                    logger.LogInformation("{0}: Found and instantiated", pluginType.FullName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error loading plugin {0}", config.Name);
                }

            // If no plugins were added via discovery, attempt a best-effort fallback: load DLLs from candidate roots and search for types
            if (!addedAny && pluginList.Any())
            {
                logger.LogDebug("No plugins added via primary discovery, attempting fallback DLL scan");
                foreach (var root in candidateRoots)
                    try
                    {
                        foreach (var file in Directory.EnumerateFiles(root!, "*.dll", SearchOption.TopDirectoryOnly))
                            try
                            {
                                // skip already loaded assemblies
                                var loaded = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a =>
                                    string.Equals(a.Location, file, StringComparison.OrdinalIgnoreCase));
                                var asm = loaded ?? loadContext.LoadFromAssemblyPath(file);

                                foreach (var config in pluginList.Where(p => p.IsActive))
                                    try
                                    {
                                        var candidateType = GetLoadableTypes(asm).FirstOrDefault(t =>
                                            typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface &&
                                            (t.Namespace == config.Name ||
                                             (t.Namespace is not null && t.Namespace.StartsWith(config.Name,
                                                 StringComparison.OrdinalIgnoreCase)) ||
                                             t.FullName?.StartsWith(config.Name) == true ||
                                             t.Name == config.Name.Split('.').Last()));
                                        if (candidateType is not null)
                                        {
                                            var plugin = (IPlugin)Activator.CreateInstance(candidateType)!;
                                            plugin.Name = config.Name;
                                            plugin.IsActive = config.IsActive;
                                            discoveredPlugins.Add(plugin);
                                            logger.LogInformation("Loaded plugin {0} from file {1}",
                                                candidateType.FullName, file);
                                        }
                                    }
                                    catch
                                    {
                                    }
                            }
                            catch
                            {
                            }
                    }
                    catch
                    {
                    }
            }

            logger.LogInformation("Plugin discovery completed. {Count} plugins discovered.", discoveredPlugins.Count);
        }
        catch (Exception ex)
        {
            // Handle the exception if the assembly cannot be loaded
            logger.LogError("Error during plugin discovery: {0}", ex.Message);
        }

        return discoveredPlugins;
    }

    /// <summary>
    ///     Parses plugin configuration from IConfiguration, handling multiple configuration formats.
    /// </summary>
    private static List<PluginConfig> ParsePluginConfiguration(IConfiguration? configuration, ILogger logger)
    {
        var pluginList = new List<PluginConfig>();

        if (configuration is null)
        {
            logger.LogDebug("Configuration is null - no plugins to discover");
            return pluginList;
        }

        logger.LogDebug("Configuration is available, attempting to parse plugin configuration");

        // Primary attempt: bind to section "Plugins" => [ { Name, IsActive }, ... ]
        var section = configuration.GetSection(PluginOptions.Name);
        var primaryList = section.Get<List<PluginConfig>>() ?? new List<PluginConfig>();
        logger.LogDebug("Primary binding attempt: {Count} plugins", primaryList.Count);

        // Log what was bound
        foreach (var p in primaryList)
            logger.LogDebug("  Primary bound: Name='{Name}', IsActive={IsActive}", p.Name ?? "(null)", p.IsActive);

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
                logger.LogDebug("  Nested bound: Name='{Name}', IsActive={IsActive}", p.Name ?? "(null)", p.IsActive);

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
            var pluginKeys =
                all.Where(kv => kv.Key.StartsWith(PluginOptions.Name + ":", StringComparison.OrdinalIgnoreCase))
                    .ToList();
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
                    logger.LogDebug("  Detected nested format - Index: '{Index}', Field: '{Field}'", indexKey,
                        fieldName);
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
                    logger.LogDebug("  Using fallback format - Index: '{Index}', Field: '{Field}'", indexKey,
                        fieldName);
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
            logger.LogDebug("Skipping manual parsing - already have {Count} valid plugins from binding",
                pluginList.Count);
        }

        return pluginList;
    }
}