namespace LowlandTech.Plugins;

/// <summary>
/// Represents a namespace for permissions, providing a prefix and validation functionality.
/// </summary>
public interface IPluginPermission
{
    /// <summary>
    /// Gets the prefix string used to identify account-related keys.
    /// </summary>
    /// <example>
    /// <code>
    /// var prefix = "accounts::";
    /// </code>
    /// </example>
    string Prefix { get; } 

    /// <summary>
    /// Determines whether the specified policy name is valid.
    /// </summary>
    /// <param name="policyName">The name of the policy to validate. Cannot be null or empty.</param>
    /// <returns><see langword="true"/> if the policy name is valid; otherwise, <see langword="false"/>.</returns>
    bool IsValid(string policyName);
}