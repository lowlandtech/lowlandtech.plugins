namespace LowlandTech.Plugins.Middleware;

/// <summary>
/// Represents a behavior in the request handling pipeline that manages a unit of work.
/// </summary>
/// <remarks>This behavior ensures that any changes made to the database context during the handling of a request
/// are committed once the request is processed. It is typically used to wrap request handlers that perform operations
/// on a database, ensuring that all changes are saved atomically.</remarks>
/// <typeparam name="TReq">The type of the request.</typeparam>
/// <typeparam name="TRes">The type of the response.</typeparam>
public sealed class UnitOfWorkBehavior<TReq, TRes>(DbContext db) : IPipelineBehavior<TReq, TRes>
{
    /// <summary>
    /// Represents the database context used for accessing and managing the database.
    /// </summary>
    /// <remarks>This field is read-only and is intended to be used internally for database
    /// operations.</remarks>
    private readonly DbContext _db = db;

    /// <summary>
    /// Processes the request and ensures that any changes are saved to the database.
    /// </summary>
    /// <remarks>This method calls the next handler in the pipeline and then saves any changes to the
    /// database. Ensure that the database context is properly configured to handle the changes.</remarks>
    /// <param name="request">The request object containing the data to be processed.</param>
    /// <param name="next">The delegate to invoke the next handler in the pipeline.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with a result of type <typeparamref name="TRes"/>.</returns>
    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken ct)
    {
        var res = await next(ct);
        await _db.SaveChangesAsync(ct);
        return res;
    }
}