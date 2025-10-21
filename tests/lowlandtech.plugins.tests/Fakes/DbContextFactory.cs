
namespace LowlandTech.Plugins.Tests.Fakes;

/// <summary>
/// Provides a test-friendly implementation of <see cref="IDbContextFactory{TContext}"/>.
/// </summary>
/// <remarks>
/// This factory wraps a pre-configured DbContext instance (typically in-memory)
/// and returns it on every CreateDbContext call. This enables tests to control
/// the database state and verify side effects without relying on real database connections.
/// </remarks>
/// <typeparam name="TContext">The DbContext type to be created.</typeparam>
/// <param name="db">The pre-configured DbContext instance to return.</param>
public class DbContextFactory<TContext>(TContext db) : IDbContextFactory<TContext> where TContext : DbContext
{
    /// <summary>
    /// Creates and returns the pre-configured DbContext instance.
    /// </summary>
    /// <returns>The DbContext instance provided during construction.</returns>
    public TContext CreateDbContext()
    {
        return db;
    }

    /// <summary>
    /// Asynchronously creates and returns the pre-configured DbContext instance.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token (not used in this implementation).</param>
    /// <returns>A task representing the asynchronous operation, containing the DbContext instance.</returns>
    public Task<TContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(db);
    }
}
