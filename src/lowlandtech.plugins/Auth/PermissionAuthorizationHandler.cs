namespace LowlandTech.Plugins.Auth;

/// <summary>
/// Handles authorization requirements based on user permissions.
/// </summary>
/// <remarks>This handler checks if the current user has a claim matching the required permission policy. If a
/// matching claim is found, the requirement is marked as succeeded.</remarks>
public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    /// <summary>
    /// Handles the authorization requirement by checking if the user possesses the specified permission.
    /// </summary>
    /// <param name="ctx">The authorization handler context containing the user and resource information.</param>
    /// <param name="req">The permission requirement that specifies the policy to be checked against the user's claims.</param>
    /// <returns>A completed task that represents the asynchronous operation.</returns>
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext ctx, PermissionRequirement req)
    {
        foreach (var c in ctx.User.FindAll("perm"))
        {
            if (StringComparer.OrdinalIgnoreCase.Equals(c.Value, req.Policy))
            {
                ctx.Succeed(req);
                break;
            }
        }
        return Task.CompletedTask;
    }
}