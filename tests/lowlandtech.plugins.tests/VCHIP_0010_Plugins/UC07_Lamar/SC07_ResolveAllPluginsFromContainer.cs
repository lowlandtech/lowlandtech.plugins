using Lamar;
using LowlandTech.Plugins.Tests.Fixtures;
using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC07_Lamar;

[Scenario(
    specId: "VCHIP-0010-UC07-SC07",
    title: "Resolve all plugins from Lamar container",
    given: "Given multiple plugins are registered in ServiceRegistry",
    when: "When the container is built and GetAllInstances<IPlugin>() is called",
    then: "Then all registered plugin instances should be returned")]
public sealed class SC07_ResolveAllPluginsFromContainer : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private ServiceRegistry? _services;
    private IContainer? _container;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceRegistry();
        _services.AddPlugin(new TestLifecyclePluginLamar());
        _services.AddPlugin(new IndependentPlugin());
    }

    protected override void When()
    {
        _container = new Container(_services);
    }

    [Fact]
    [Then("all registered plugin instances should be returned", "UAC018")]
    public void All_Plugins_Returned()
    {
        var plugins = _container!.GetAllInstances<IPlugin>().ToList();
        plugins.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    [Then("the collection should contain all active plugins", "UAC019")]
    public void Only_Active_Plugins()
    {
        var plugins = _container!.GetAllInstances<IPlugin>().ToList();
        plugins.ShouldNotBeEmpty();
        plugins.All(p => p.IsActive).ShouldBeTrue();
    }
}
