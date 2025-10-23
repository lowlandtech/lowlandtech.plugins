namespace LowlandTech.Plugins.AspNetCore.Extensions;

/// <summary>
/// Configures standard problem details for the application, including exception mapping and detail inclusion based on
/// the environment.
/// </summary>
/// <remarks>This method sets up problem details to include exception details only in development environments. It
/// maps <see cref="UnauthorizedAccessException"/> to a 401 Unauthorized status code and <see cref="TimeoutException"/>
/// to a 504 Gateway Timeout status code.</remarks>
public static class ProblemDetailsConfig
{
    /// <summary>
    /// Configures the application to use standardized problem details for error responses.
    /// </summary>
    /// <remarks>This method sets up problem details to include exception details in development environments
    /// and maps specific exceptions to HTTP status codes. Unauthorized access exceptions are mapped to 401
    /// Unauthorized, and timeout exceptions are mapped to 504 Gateway Timeout.</remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the problem details configuration to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> with problem details configured.</returns>
    public static IServiceCollection AddStandardProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(o =>
        {
            o.IncludeExceptionDetails = (ctx, ex) =>
                ctx.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment();

            o.MapToStatusCode<UnauthorizedAccessException>(StatusCodes.Status401Unauthorized);
            o.MapToStatusCode<TimeoutException>(StatusCodes.Status504GatewayTimeout);
        });
        return services;
    }
}