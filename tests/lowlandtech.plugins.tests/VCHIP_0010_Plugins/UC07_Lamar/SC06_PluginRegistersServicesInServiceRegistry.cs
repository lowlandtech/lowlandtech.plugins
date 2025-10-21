using Lamar;
using LowlandTech.Plugins.Tests.Fixtures;
using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC07_Lamar;

[Scenario(
    specId: "VCHIP-0010-UC07-SC06",
    title: "Plugin registers services in ServiceRegistry",
    given: "Given a plugin needs to register services",
    when: "When the plugin Install method is called with ServiceRegistry",
    then: "Then the plugin should be able to use Lamar-specific registration syntax")]
public sealed class SC06_PluginRegistersServicesInServiceRegistry : WhenTestingForV2<ErrorHandlingTestFixture>
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
        var plugin = new LamarRegisteringPlugin();
        // the plugin will use ServiceRegistry.For<T>().Use<TImpl>() in Install
        _services!.AddPlugin(plugin);
        _container = new Container(configure =>
        {
            configure.IncludeRegistry(_services);
        });
    }

    [Fact]
    [Then("the plugin should be able to use Lamar-specific registration syntax", "UAC015")]
    public void Plugin_Can_Register_Lamar_Services()
    {
        var svc = _container!.GetInstance<ILamarService>();
        svc.ShouldNotBeNull();
        svc.GetMessage().ShouldBe("Lamar service");
    }

    [Fact]
    [Then("the plugin should register services using For<T>().Use<TImpl>()", "UAC016")]
    public void Registered_With_For_Use()
    {
        var svc = _container!.TryGetInstance<ILamarService>();
        svc.ShouldNotBeNull();
    }

    [Fact]
    [Then("services should be available after container is built", "UAC017")]
    public void Services_Available_After_Build()
    {
        var svc = _container!.TryGetInstance<ILamarService>();
        svc.ShouldNotBeNull();
        svc.GetMessage().ShouldBe("Lamar service");
    }
}
