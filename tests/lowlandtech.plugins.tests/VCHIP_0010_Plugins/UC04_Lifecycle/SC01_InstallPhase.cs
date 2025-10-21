using LowlandTech.Plugins.Tests.Fixtures;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

[Scenario(
    specId: "VCHIP-0010-UC04-SC01",
    title: "Execute Install phase during plugin registration",
    given: "Given a plugin is being registered in the service collection",
    when: "When the Install method is called",
    then: "Then the plugin should register its services in the service collection")]
public sealed class SC01_InstallPhase : WhenTestingForV2<LifecycleTestFixture>
{
    private IServiceCollection? _services;
    private TestLifecyclePlugin? _plugin;
    private IServiceProvider? _provider;

    protected override LifecycleTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _plugin = new TestLifecyclePlugin();
    }

    protected override void When()
    {
        _services!.AddPlugin(_plugin!);
        _provider = _services.BuildServiceProvider();
    }

    [Fact]
    [Then("The plugin should register its services in the service collection", "UAC001")]
    public void Plugin_Should_Register_Services()
    {
        // The test plugin registers a singleton service in Install
        _plugin!.InstallCalled.ShouldBeTrue();
        
        var testService = _provider!.GetService<TestPluginService>();
        testService.ShouldNotBeNull();
    }

    [Fact]
    [Then("The plugin services should be available for dependency injection", "UAC002")]
    public void Plugin_Services_Should_Be_Available()
    {
        var testService = _provider!.GetService<TestPluginService>();
        testService.ShouldNotBeNull();
        testService.Message.ShouldBe("Test service registered during Install");
    }

    [Fact]
    [Then("The Install method should complete successfully", "UAC003")]
    public void Install_Should_Complete_Successfully()
    {
        _plugin!.InstallCalled.ShouldBeTrue();
        _plugin.InstallException.ShouldBeNull();
    }
}
