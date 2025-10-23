namespace LowlandTech.Plugins.Middleware;

/// <summary>
/// Represents a behavior in the pipeline that processes a request and response.
/// </summary>
/// <typeparam name="TReq">The type of the request.</typeparam>
/// <typeparam name="TRes">The type of the response.</typeparam>
public sealed class ValidationBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes> where TRes : class where TReq : notnull
{   
    /// <summary>
    /// Processes the specified request asynchronously and returns the result.
    /// </summary>
    /// <param name="request">The request object to be processed.</param>
    /// <param name="next">A delegate that represents the next handler in the request processing pipeline.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the request processing.</returns>
    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken ct) => await next(ct);
}