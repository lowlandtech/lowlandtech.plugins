namespace LowlandTech.Plugins;

/// <summary>
/// Represents a catalog of permissions, providing access to all available permissions and a prefix identifier.
/// </summary>
/// <remarks>This interface is typically used to manage and access a collection of permissions within a specific
/// domain or module. The <see cref="Prefix"/> property can be used to identify the domain or module associated with the
/// permissions.</remarks>
public interface IPluginPermissionCatalog
{
    /// <summary>
    /// Gets the prefix used for categorizing or identifying related items.
    /// </summary>
    string Prefix { get; }                 // e.g., "accounts"

    /// <summary>
    /// Gets a read-only list of all available permissions.
    /// </summary>
    IReadOnlyList<Permission> All { get; } // discoverable, used for seeding/UI
}

/// <summary>
/// Represents a permission with a specific prefix, resource, and action.
/// </summary>
/// <remarks>This struct provides a way to define and manipulate permissions in a structured format. The <see
/// cref="Policy"/> property combines the prefix, resource, and action into a single string representation, which can be
/// used for policy evaluation or logging.</remarks>
/// <param name="Prefix">The prefix used to categorize the permission.</param>
/// <param name="Resource">The resource the permission applies to.</param>
/// <param name="Action">The action that can be performed on the resource.</param>
/// <example>
/// The following example demonstrates how to create a permission for the "accounts" prefix, "user" resource, and "read" action.
/// <code>
/// var permission = new Permission("accounts", "user", "read");
/// </code>
/// </example>
public readonly record struct Permission(string Prefix, string Resource, string Action)
{
    /// <summary>
    /// Gets the policy string that uniquely identifies the combination of prefix, resource, and action.
    /// </summary>
    public string Policy => $"{Prefix}::{Resource}::{Action}";

    /// <summary>
    /// Returns a string representation of the current policy.
    /// </summary>
    /// <returns>A string that represents the current policy.</returns>
    public override string ToString() => Policy;

    /// <summary>
    /// Converts a <see cref="Permission"/> object to its string representation.
    /// </summary>
    /// <param name="p">The <see cref="Permission"/> instance to convert.</param>
    public static implicit operator string(Permission p) => p.Policy;
}