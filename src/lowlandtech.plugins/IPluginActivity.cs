namespace LowlandTech.Plugins;

/// <summary>
/// Represents an activity that can be executed asynchronously by a plugin.
/// </summary>
/// <remarks>This interface defines a contract for implementing plugin activities that perform asynchronous
/// operations. Implementations of this interface should provide the logic for the activity within the <see
/// cref="ExecuteAsync"/> method.</remarks>
public interface IPluginActivity
{
    /// <summary>
    /// Executes an asynchronous operation.
    /// </summary>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ExecuteAsync(CancellationToken ct);
}
