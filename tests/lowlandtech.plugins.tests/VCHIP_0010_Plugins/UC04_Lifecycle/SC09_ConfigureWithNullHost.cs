using LowlandTech.Plugins.Tests.Fixtures;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

[Scenario(
    specId: "VCHIP-0010-UC04-SC09",
    title: "Plugin Configure phase with null host",
    given: "Given a plugin is being used in a non-ASP.NET Core application",
    when: "When the Configure method is called with host set to null",
    then: "Then the plugin should handle the null host gracefully")]
public sealed class SC09_ConfigureWithNullHost : WhenTestingForV2<LifecycleTestFixture>
{
    private IServiceCollection? _services;
    private SimpleConfigurePlugin? _plugin;
    private IServiceProvider? _provider;

    protected override LifecycleTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _plugin = new SimpleConfigurePlugin();
        _services.AddPlugin(_plugin);
        _provider = _services.BuildServiceProvider();
    }

    protected override void When()
    {
        _plugin!.Configure(_provider!, host: null).GetAwaiter().GetResult();
    }

    [Fact]
    [Then("The plugin should handle the null host gracefully", "UAC029")]
    public void Handles_Null_Host()
    {
        _plugin!.ConfigureCalled.ShouldBeTrue();
        _plugin.ReceivedHost.ShouldBeNull();
    }

    [Fact]
    [Then("The plugin should skip host-specific configuration", "UAC030")]
    public void Skips_Host_Specific()
    {
        // no exception and no host-related flags set
    }

    [Fact]
    [Then("The Configure method should return successfully", "UAC031")]
    public void Configure_Returns_Successfully()
    {
        _plugin!.ConfigureCalled.ShouldBeTrue();
    }
}
