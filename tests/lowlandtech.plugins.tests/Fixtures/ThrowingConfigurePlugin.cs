namespace LowlandTech.Plugins.Tests.Fixtures;

[PluginId("ea666666-0000-4000-8000-000000000006")]
public class ThrowingConfigurePlugin : Plugin
{
    public override void Install(IServiceCollection services)
    {
        // normal install
    }

    public override Task ConfigureContext(IServiceCollection services)
    {
        return Task.CompletedTask;
    }

    public override Task Configure(IServiceProvider container, object? host = null)
    {
        throw new InvalidOperationException("Configure failed");
    }
}