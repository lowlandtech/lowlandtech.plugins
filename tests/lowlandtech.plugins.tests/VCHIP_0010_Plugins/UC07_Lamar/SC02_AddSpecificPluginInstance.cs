using Lamar;
using LowlandTech.Plugins.Tests.Fixtures;
using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC07_Lamar;

[Scenario(
    specId: "VCHIP-0010-UC07-SC02",
    title: "Add a specific plugin instance to ServiceRegistry",
    given: "Given I have a plugin instance",
    when: "When I call services.AddPlugin(pluginInstance) on the ServiceRegistry",
    then: "Then the plugin should be registered in the ServiceRegistry")]
public sealed class SC02_AddSpecificPluginInstance : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private ServiceRegistry? _services;
    private IContainer? _container;
    private TestLifecyclePluginLamar? _plugin;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceRegistry();
        _plugin = new TestLifecyclePluginLamar();
    }

    protected override void When()
    {
        _services!.AddPlugin(_plugin!);
        _container = new Container(_services);
    }

    [Fact]
    [Then("the plugin should be registered in the ServiceRegistry", "UAC004")]
    public void Plugin_Registered() => _container.ShouldNotBeNull();

    [Fact]
    [Then("the plugin Install method should be called", "UAC005")]
    public void Install_Called() => _plugin!.InstallCalled.ShouldBeTrue();

    [Fact]
    [Then("the plugin should be available in the container", "UAC006")]
    public void Plugin_Available_In_Container() => _container!.GetAllInstances<IPlugin>().ShouldNotBeNull().ShouldNotBeEmpty();
}
