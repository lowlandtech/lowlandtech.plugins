namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[PluginId("ea222222-0000-4000-8000-000000000002")]
public class ConstructorThrowingPlugin : Plugin
{
    public ConstructorThrowingPlugin()
    {
        throw new InvalidOperationException("Constructor failed");
    }

    public override void Install(IServiceCollection services)
    {
    }

    public override Task Configure(IServiceProvider container, object? host = null)
    {
        return Task.CompletedTask;
    }
}