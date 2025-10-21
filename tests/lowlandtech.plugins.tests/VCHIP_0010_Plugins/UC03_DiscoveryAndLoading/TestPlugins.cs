namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC03_DiscoveryAndLoading;

// Test stub plugins for discovery tests
[PluginId("a1b2c3d4-e5f6-4789-a0b1-c2d3e4f5a6b7")]
public class FrontendDiscoveryPlugin : Plugin
{
    public override void Install(IServiceCollection services)
    {
        // Test stub - no services to install
    }

    public override Task Configure(IServiceProvider provider, object? host = null)
    {
        return Task.CompletedTask;
    }
}

[PluginId("f7e8d9c0-b1a2-4536-9f8e-7d6c5b4a3210")]
public class ReportingDiscoveryPlugin : Plugin
{
    public override void Install(IServiceCollection services)
    {
        // Test stub - no services to install
    }

    public override Task Configure(IServiceProvider provider, object? host = null)
    {
        return Task.CompletedTask;
    }
}
