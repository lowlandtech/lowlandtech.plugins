namespace LowlandTech.Plugins.Tests.Fixtures;

[PluginId("ea444444-0000-4000-8000-000000000004")]
public class ConfigureContextThrowingPlugin : Plugin
{
    public bool ConfigureCalled { get; private set; }

    public override void Install(IServiceCollection services)
    {
    }

    public override Task ConfigureContext(IServiceCollection services)
    {
        throw new InvalidOperationException("ConfigureContext failed");
    }

    public override Task Configure(IServiceProvider provider, object? host = null)
    {
        ConfigureCalled = true;
        return Task.CompletedTask;
    }
}