namespace LowlandTech.Plugins.Tests.Fixtures;

/// <summary>
/// Plugin with a throwing property getter to test error handling.
/// </summary>
[PluginId("f9999999-0000-4000-8000-000000000099")]
public class FaultyPropertyPlugin : Plugin
{
    public override void Install(IServiceCollection services) { }
    
    public override Task Configure(IServiceProvider provider, object? host = null) => Task.CompletedTask;

    // Hide base Name property and simulate a throwing getter
    public new string Name => throw new InvalidOperationException("Property getter failed");
}
