namespace LowlandTech.Plugins;

/// <summary>
/// Defines a mechanism for resolving the base URI of an identity service endpoint.
/// </summary>
/// <remarks>Implementations of this interface provide a way to obtain the base URI for identity-related services.
/// This can be useful in scenarios where the endpoint may vary based on environment or configuration.</remarks>
public interface IEndpointResolver
{
    /// <summary>
    /// Retrieves the base URI for the identity service.
    /// </summary>
    /// <returns>A <see cref="Uri"/> representing the base address of the identity service.</returns>
    Uri GetIdentityBaseUri(); // add more service getters as you grow
}

/// <summary>
/// Resolves the base URI for the identity service from the configuration.
/// </summary>
/// <remarks>This class retrieves the base URI for the identity service using the provided configuration. It
/// expects the configuration to contain a key "Services:Identity:BaseUrl".</remarks>
public sealed class ConfigEndpointResolver : IEndpointResolver
{
    /// <summary>
    /// Represents the configuration settings for the application.
    /// </summary>
    /// <remarks>This field holds a reference to an <see cref="IConfiguration"/> instance, which provides
    /// access to configuration values. It is intended for internal use to manage application settings.</remarks>
    private readonly IConfiguration _cfg;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigEndpointResolver"/> class with the specified configuration.
    /// </summary>
    /// <param name="cfg">The configuration instance used to resolve endpoints.</param>
    public ConfigEndpointResolver(IConfiguration cfg) => _cfg = cfg;

    /// <summary>
    /// Retrieves the base URI for the identity service.
    /// </summary>
    /// <returns>A <see cref="Uri"/> representing the base URL of the identity service.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the configuration key "Services:Identity:BaseUrl" is missing.</exception>
    public Uri GetIdentityBaseUri()
        => new(_cfg["Services:Identity:BaseUrl"]
               ?? throw new InvalidOperationException("Services:Identity:BaseUrl missing"));
}