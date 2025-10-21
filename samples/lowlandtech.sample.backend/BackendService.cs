namespace LowlandTech.Sample.Backend;

public class BackendActivity : IPluginActivity
{
    public Task ExecuteAsync()
    {
        Console.Write("Executing backend activity...");
        return Task.CompletedTask;
    }
}