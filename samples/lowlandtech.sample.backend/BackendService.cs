namespace LowlandTech.Sample.Backend;

public class BackendActivity : IPluginActivity
{
    public Task ExecuteAsync(CancellationToken ct)
    {
        Console.Write("Executing backend activity...");
        return Task.CompletedTask;
    }
}