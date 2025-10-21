namespace LowlandTech.Plugins.Tests.Fixtures;

/// <summary>
/// Test service registered by plugins during Install phase.
/// </summary>
public class TestPluginService
{
    public string Message { get; } = "Test service registered during Install";
}