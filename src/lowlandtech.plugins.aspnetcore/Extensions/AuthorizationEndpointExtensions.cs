namespace LowlandTech.Plugins.AspNetCore.Extensions;

/// <summary>
/// Provides extension methods for configuring authorization on endpoint builders.
/// </summary>
/// <remarks>This class contains methods that extend endpoint builders to require specific permissions for
/// accessing endpoints. It is designed to be used with types implementing the  <see cref="IEndpointConventionBuilder"/>
/// interface.</remarks>
public static class AuthorizationEndpointExtensions
{
    /// <summary>
    /// Adds authorization requirements to the endpoint using the specified permissions.
    /// </summary>
    /// <typeparam name="T">The type of the endpoint convention builder.</typeparam>
    /// <param name="builder">The endpoint convention builder to which the authorization requirements are added.</param>
    /// <param name="perms">An array of permissions that define the authorization policies to be required.</param>
    /// <returns>The modified endpoint convention builder with the added authorization requirements.</returns>
    public static T RequirePermissions<T>(this T builder, params Permission[] perms)
        where T : IEndpointConventionBuilder
    {
        foreach (var p in perms) builder.RequireAuthorization(p.Policy);
        return builder;
    }
}