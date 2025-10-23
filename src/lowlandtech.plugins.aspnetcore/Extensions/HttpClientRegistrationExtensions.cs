namespace LowlandTech.Plugins.AspNetCore.Extensions;

/// <summary>
/// Provides extension methods for registering HTTP clients with resilience and configuration options.
/// </summary>
/// <remarks>This class contains methods to facilitate the registration of HTTP clients with specific
/// configurations and resilience policies, such as retry and circuit breaker mechanisms, using the <see
/// cref="IServiceCollection"/>.</remarks>
public static class HttpClientRegistrationExtensions
{
    /// <summary>
    /// Adds an HTTP client for a specified API client interface and implementation to the service collection.
    /// </summary>
    /// <remarks>The method configures the HTTP client with a default timeout of 15 seconds and a pooled
    /// connection lifetime of 2 minutes. It also sets up a standard resilience handler with retry and circuit breaker
    /// policies.</remarks>
    /// <typeparam name="TClient">The interface type of the API client.</typeparam>
    /// <typeparam name="TImpl">The implementation type of the API client.</typeparam>
    /// <param name="services">The service collection to which the HTTP client is added.</param>
    /// <param name="baseUri">A function that resolves the base URI for the API using an <see cref="IEndpointResolver"/>.</param>
    /// <param name="basePath">An optional base path to append to the base URI. Can be <see langword="null"/>.</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to further configure the HTTP client.</returns>
    public static IHttpClientBuilder AddEntityApi<TClient, TImpl>(
        this IServiceCollection services,
        Func<IEndpointResolver, Uri> baseUri,
        string? basePath = null)
        where TClient : class
        where TImpl : class, TClient
    {
        var builder = services.AddHttpClient<TClient, TImpl>((sp, c) =>
            {
                var resolver = sp.GetRequiredService<IEndpointResolver>();
                var root = baseUri(resolver);

                if (!string.IsNullOrWhiteSpace(basePath))
                    c.BaseAddress = new Uri(root, basePath.TrimStart('/'));
                else
                    c.BaseAddress = root;

                c.Timeout = TimeSpan.FromSeconds(15);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                AllowAutoRedirect = false
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5));

        // This returns IHttpStandardResiliencePipelineBuilder (different type)
        builder.AddStandardResilienceHandler(o =>
        {
            o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(5);
            o.Retry.MaxRetryAttempts = 3;
            o.Retry.UseJitter = true;
            o.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
            o.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(20);
            o.CircuitBreaker.FailureRatio = 0.5;
        });

        return builder; // return the IHttpClientBuilder, not the pipeline builder
    }
}