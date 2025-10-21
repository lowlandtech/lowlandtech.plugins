namespace LowlandTech.Plugins.Types;

/// <summary>
/// Sets the identifier for a plugin.
/// </summary>
/// <param name="id"></param>
[AttributeUsage(AttributeTargets.Class)]
public class PluginId(string id) : Attribute
{
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    public string Id => id;
}