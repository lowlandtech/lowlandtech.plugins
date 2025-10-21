namespace LowlandTech.Plugins;

/// <summary>
/// Contract for a plugin.
/// </summary>
public interface IPlugin
{
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the active state.
    /// </summary>
    bool IsActive { get; set; }

    /// <summary>
    /// Gets the description.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets the company.
    /// </summary>
    public string? Company { get; }

    /// <summary>
    /// Gets the company URL.
    /// </summary>
    public string? Copyright { get; }

    /// <summary>
    /// Gets the URL.
    /// </summary>
    public string? Url { get; }

    /// <summary>
    /// Gets the version.
    /// </summary>
    public string? Version { get; }

    /// <summary>
    /// Gets the authors.
    /// </summary>
    public string? Authors { get; }

    /// <summary>
    /// Gets the assemblies.
    /// </summary>
    public List<Assembly>? Assemblies { get; }

    /// <summary>
    /// Installs the plugin.
    /// </summary>
    /// <param name="services"></param>
    void Install(IServiceCollection services);

    /// <summary>
    /// Configures the application context by registering services into the provided service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which services will be added. Cannot be null.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ConfigureContext(IServiceCollection services);

    /// <summary>
    /// Configures the plugin.
    /// </summary>
    /// <param name="container">The ioc container.</param>
    /// <param name="host">The webapplication or mauiapp</param>
    Task Configure(IServiceProvider container, object? host = null);
}