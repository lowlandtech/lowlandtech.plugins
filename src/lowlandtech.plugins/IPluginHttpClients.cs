namespace LowlandTech.Plugins;

/// <summary>
/// Provides a mechanism to register HTTP clients for a module within the application's service collection.
/// </summary>
/// <remarks>This interface is intended to be implemented by modules that require HTTP client configurations.
/// Implementations should define how HTTP clients are registered and configured using the provided <see
/// cref="IServiceCollection"/> and <see cref="IConfiguration"/>.</remarks>
public interface IPluginHttpClients
{
    /// <summary>
    /// Registers services and configuration settings into the specified service collection.
    /// </summary>
    /// <remarks>This method is typically called during application startup to configure dependency injection.
    /// Ensure that both <paramref name="services"/> and <paramref name="cfg"/> are properly initialized before calling
    /// this method.</remarks>
    /// <param name="services">The service collection to which services will be added. Cannot be null.</param>
    /// <param name="cfg">The configuration settings to be used for service registration. Cannot be null.</param>
    void Register(ServiceRegistry services, IConfiguration cfg);
}
