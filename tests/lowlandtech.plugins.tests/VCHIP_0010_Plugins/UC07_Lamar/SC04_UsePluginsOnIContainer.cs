using Lamar;
using LowlandTech.Plugins.Tests.Fixtures;
using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC07_Lamar;

[Scenario(
    specId: "VCHIP-0010-UC07-SC04",
    title: "UsePlugins on Lamar IContainer",
    given: "Given plugins have been registered in ServiceRegistry",
    when: "When I call container.UsePlugins()",
    then: "Then all registered plugins should be retrieved from the container")]
public sealed class SC04_UsePluginsOnIContainer : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private ServiceRegistry? _services;
    private IContainer? _container;
    private TestLifecyclePluginLamar? _plugin;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceRegistry();
        _plugin = new TestLifecyclePluginLamar();
        _services.AddPlugin(_plugin);
    }

    protected override void When()
    {
        _container = new Container(_services);
        _container.UsePlugins().GetAwaiter().GetResult();
    }

    [Fact]
    [Then("all registered plugins should be retrieved from the container", "UAC010")]
    public void Plugins_Retrieved() => _container!.GetAllInstances<IPlugin>().ShouldNotBeNull().ShouldNotBeEmpty();

    [Fact]
    [Then("the Configure method should be called on each plugin", "UAC011")]
    public void Configure_Called_On_Each() => _plugin!.ConfigureCalled.ShouldBeTrue();

    [Fact]
    [Then("each plugin should receive the IContainer as service provider", "UAC012")]
    public void Plugin_Receives_Container() => _plugin!.ServiceProviderReceived.ShouldNotBeNull();
}
