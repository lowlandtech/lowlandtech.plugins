namespace LowlandTech.Plugins.Tests.Fixtures;

/// <summary>
/// Simple plugin that records Configure being called (used by SC08).
/// </summary>
[PluginId("d4444444-4444-4444-8444-444444444444")]
public class SimpleConfigurePlugin : Plugin
{
    public bool InstallCalled { get; private set; }
    public bool ConfigureCalled { get; private set; }
    public IServiceProvider? ReceivedProvider { get; private set; }
    public object? ReceivedHost { get; private set; }

    public override void Install(IServiceCollection services)
    {
        InstallCalled = true;
    }

    public override Task ConfigureContext(IServiceCollection services)
    {
        return Task.CompletedTask;
    }

    public override Task Configure(IServiceProvider container, object? host = null)
    {
        ReceivedProvider = container;
        ReceivedHost = host;
        ConfigureCalled = true;
        return Task.CompletedTask;
    }
}