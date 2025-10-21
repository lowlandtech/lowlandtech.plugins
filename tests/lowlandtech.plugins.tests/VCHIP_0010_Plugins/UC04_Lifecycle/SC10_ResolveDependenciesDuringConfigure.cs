using LowlandTech.Plugins.Tests.Fixtures;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

[Scenario(
    specId: "VCHIP-0010-UC04-SC10",
    title: "Plugin accesses dependencies from service provider during Configure",
    given: "Given a plugin has registered services during Install and the service provider has been built with those services",
    when: "When the Configure method is called with the service provider",
    then: "Then the plugin should be able to resolve its registered services")]
public sealed class SC10_ResolveDependenciesDuringConfigure : WhenTestingForV2<LifecycleTestFixture>
{
    private IServiceCollection? _services;
    private TestLifecyclePlugin? _plugin;
    private IServiceProvider? _provider;

    protected override LifecycleTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _plugin = new TestLifecyclePlugin();
        _services.AddPlugin(_plugin);
        _provider = _services.BuildServiceProvider();
    }

    protected override void When()
    {
        _plugin!.Configure(_provider!, host: null).GetAwaiter().GetResult();
    }

    [Fact]
    [Then("The plugin should be able to resolve its registered services", "UAC032")]
    public void Resolves_Registered_Services()
    {
        var svc = _plugin!.ServiceProviderReceived!.GetService<TestPluginService>();
        svc.ShouldNotBeNull();
    }

    [Fact]
    [Then("The plugin should be able to use those services for configuration", "UAC033")]
    public void Uses_Resolved_Services()
    {
        var svc = _plugin!.ServiceProviderReceived!.GetService<TestPluginService>();
        svc!.Message.ShouldBe("Test service registered during Install");
    }

    [Fact]
    [Then("All service resolutions should succeed", "UAC034")]
    public void All_Resolutions_Succeed()
    {
        _plugin!.ServiceProviderReceived!.GetService<TestPluginService>().ShouldNotBeNull();
    }
}
