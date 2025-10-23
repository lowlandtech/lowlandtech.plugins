namespace LowlandTech.Plugins.Auth;

/// <summary>
/// Provides authorization policies that are aware of plugin-specific requirements.
/// </summary>
/// <remarks>This provider attempts to parse policy names with a specific format to create custom authorization
/// policies. If the policy name does not match the expected format, it falls back to the default policy
/// provider.</remarks>
public sealed class PluginAwarePolicyProvider : IAuthorizationPolicyProvider
{
    /// <summary>
    /// Represents the default authorization policy provider used as a fallback.
    /// </summary>
    private readonly DefaultAuthorizationPolicyProvider _fallback;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginAwarePolicyProvider"/> class with the specified authorization
    /// options.
    /// </summary>
    /// <remarks>This constructor sets up the policy provider to use the provided authorization options as a
    /// fallback mechanism.</remarks>
    /// <param name="options">The authorization options used to configure the policy provider. Cannot be null.</param>
    public PluginAwarePolicyProvider(IOptions<AuthorizationOptions> options)
        => _fallback = new(options);

    /// <summary>
    /// Asynchronously retrieves an authorization policy based on the specified policy name.
    /// </summary>
    /// <remarks>If the policy name contains four or more colons, a new policy is constructed using the <see
    /// cref="PermissionRequirement"/> with the specified policy name. Otherwise, the method delegates to a fallback
    /// mechanism to retrieve the policy.</remarks>
    /// <param name="policyName">The name of the policy to retrieve. The policy name should follow the format "{prefix}::{resource}::{action}".</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see
    /// cref="AuthorizationPolicy"/> if the policy is found; otherwise, <see langword="null"/>.</returns>
    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Accept "{prefix}::{resource}::{action}"
        if (policyName.Count(c => c == ':') >= 4)
        {
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }
        return _fallback.GetPolicyAsync(policyName);
    }

    /// <summary>
    /// Asynchronously retrieves the default authorization policy.
    /// </summary>
    /// <remarks>This method delegates the retrieval of the default policy to a fallback mechanism.</remarks>
    /// <returns>A task that represents the asynchronous operation. The task result contains the default <see
    /// cref="AuthorizationPolicy"/>.</returns>
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();

    /// <summary>
    /// Asynchronously retrieves the fallback authorization policy.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the fallback <see
    /// cref="AuthorizationPolicy"/> if one is defined; otherwise, <see langword="null"/>.</returns>
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();
}