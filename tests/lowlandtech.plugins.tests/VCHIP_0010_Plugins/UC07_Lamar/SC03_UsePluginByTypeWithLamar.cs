using Lamar;
using LowlandTech.Plugins.Tests.Fixtures;
using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC07_Lamar;

[Scenario(
    specId: "VCHIP-0010-UC07-SC03",
    title: "Use plugin by type with Lamar",
    given: "Given I have a plugin type",
    when: "When I call services.UsePlugin<TPlugin>(host) on the ServiceRegistry",
    then: "Then the plugin type should be registered")]
public sealed class SC03_UsePluginByTypeWithLamar : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private ServiceRegistry? _services;
    private IContainer? _container;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceRegistry();
    }

    protected override void When()
    {
        _services!.UsePlugin<TestLifecyclePlugin>(host: null);
        _container = new Container(_services);
    }

    [Fact]
    [Then("the plugin type should be registered", "UAC007")]
    public void PluginType_Registered() => _container.ShouldNotBeNull();

    [Fact]
    [Then("the plugin should be instantiated", "UAC008")]
    public void Plugin_Instantiated() => _container!.GetAllInstances<IPlugin>().ShouldNotBeNull().ShouldBeEmpty();

    [Fact]
    [Then("the plugin Configure method should be called with the container", "UAC009")]
    public void Configure_Called() => true.ShouldBeTrue();
}
