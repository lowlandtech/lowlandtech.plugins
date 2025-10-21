namespace LowlandTech.Plugins.Types;

/// <summary>
/// Base class for a plugin.
/// </summary>
public abstract class Plugin : IPlugin
{
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the active state.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets the description.
    /// </summary>
    public string? Description { get; } = null;

    /// <summary>
    /// Gets the company.
    /// </summary>
    public string? Company { get; } = null;

    /// <summary>
    /// Gets the company URL.
    /// </summary>
    public string? Copyright { get; } = null;

    /// <summary>
    /// Gets the URL.
    /// </summary>
    public string? Url { get; } = null;

    /// <summary>
    /// Gets the version.
    /// </summary>
    public string? Version { get; } = null;

    /// <summary>
    /// Gets the authors.
    /// </summary>
    public string? Authors { get; } = null;

    /// <summary>
    /// Gets the assemblies.
    /// </summary>
    [JsonIgnore]
    public List<Assembly> Assemblies { get; } = new();

    /// <summary>
    /// Installs the plugin.
    /// </summary>
    /// <param name="services"></param>
    public abstract void Install(IServiceCollection services);

    /// <summary>
    /// Configures the service collection for the current context.
    /// </summary>
    /// <remarks>This method is intended to be overridden in derived classes to define specific service
    /// configurations for the application's context. The provided <paramref name="services"/> collection is typically
    /// used to register dependencies and configure services required by the application.</remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure. Must not be null.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public virtual async Task ConfigureContext(IServiceCollection services)
    {
        await Task.CompletedTask;
    }

    /// <summary>
    /// Configures the plugin.
    /// </summary>
    /// <param name="container"></param>
    /// <param name="host"></param>
    public abstract Task Configure(IServiceProvider container, object? host = null);
}
