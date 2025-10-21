namespace LowlandTech.Plugins.Extensions;

/// <summary>
/// Guard extensions.
/// </summary>
public static class GuardExtensions
{
    private const string DefaultMessage = "Invalid plugin id, it must be provided";

    /// <summary>
    /// Guard against missing plugin id.
    /// </summary>
    /// <param name="guardClause">The guard close.</param>
    /// <param name="plugin">The plugin.</param>
    /// <param name="parameterName">The parameter name</param>
    /// <param name="message">The message</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string MissingPluginId(this IGuardClause guardClause, IPlugin plugin, string parameterName, string message = DefaultMessage)
    {
        // Use the provided parameterName so ArgumentNullException.ParamName matches the caller's supplied name
        Guard.Against.Null(plugin, parameterName);

        if (Attribute.GetCustomAttribute(plugin.GetType(), typeof(PluginId)) is not PluginId pluginId)
        {
            // If caller provided a custom message (different from default), throw ArgumentException with only the message
            if (message != DefaultMessage)
            {
                throw new ArgumentException(message);
            }

            throw new ArgumentException(message, parameterName);
        }

        return pluginId.Id;
    }
}