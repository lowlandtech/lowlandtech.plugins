namespace LowlandTech.Plugins.Tests.Fixtures;

/// <summary>
/// Plugin that throws during ConfigureContext to test error handling.
/// </summary>
[PluginId("c3333333-3333-4333-8333-333333333333")]
public class ThrowingConfigureContextPlugin : Plugin
{
    public bool ConfigureCalled { get; private set; }

    public override void Install(IServiceCollection services)
    {
        // normal install
    }

    public override Task ConfigureContext(IServiceCollection services)
    {
        throw new InvalidOperationException("ConfigureContext failed");
    }

    public override Task Configure(IServiceProvider container, object? host = null)
    {
        ConfigureCalled = true;
        return Task.CompletedTask;
    }
}