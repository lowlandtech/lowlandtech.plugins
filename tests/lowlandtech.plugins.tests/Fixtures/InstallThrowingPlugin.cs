namespace LowlandTech.Plugins.Tests.Fixtures;

[PluginId("ea333333-0000-4000-8000-000000000003")]
public class InstallThrowingPlugin : Plugin
{
    public override void Install(IServiceCollection services)
    {
        throw new InvalidOperationException("Install failed intentionally");
    }

    public override Task Configure(IServiceProvider container, object? host = null)
    {
        return Task.CompletedTask;
    }
}