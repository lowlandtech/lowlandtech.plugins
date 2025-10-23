namespace LowlandTech.Plugins.Auth;

/// <summary>
/// Represents a requirement for a specific authorization policy.
/// </summary>
/// <remarks>This class is used in the context of authorization to specify that a particular policy must be
/// met.</remarks>
public sealed class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionRequirement"/> class with the specified policy.
    /// </summary>
    /// <param name="policy">The policy that defines the permission requirement. Cannot be null or empty.</param>
    public PermissionRequirement(string policy) => Policy = policy;

    /// <summary>
    /// Gets the policy name associated with the current configuration.
    /// </summary>
    public string Policy { get; }
}
