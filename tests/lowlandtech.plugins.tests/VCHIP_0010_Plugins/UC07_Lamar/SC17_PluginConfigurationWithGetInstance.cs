using Lamar;
using LowlandTech.Plugins.Tests.Fixtures;
using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC07_Lamar;

[Scenario(
    specId: "VCHIP-0010-UC07-SC17",
    title: "Plugin configuration with Lamar GetInstance",
    given: "Given a plugin needs to resolve services during Configure",
    when: "When the plugin calls container.GetInstance<TService>()",
    then: "Then the service should be resolved from the Lamar container")]
public sealed class SC17_PluginConfigurationWithGetInstance : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private ServiceRegistry? _services;
    private IContainer? _container;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceRegistry();
        // register a simple plugin that resolves a service during Configure
        _services.AddPlugin(new PluginThatResolvesService());
    }

    protected override void When()
    {
        _container = new Container(_services);
        _container.UsePlugins().GetAwaiter().GetResult();
    }

    [Fact]
    [Then("the service should be resolved from the Lamar container", "UAC043")]
    public void Service_Resolved_From_Container()
    {
        var svc = _container!.TryGetInstance<IServiceForResolve>();
        svc.ShouldNotBeNull();
        svc!.GetValue().ShouldBe("Resolved");
    }

    [Fact]
    [Then("the plugin should receive the correct instance", "UAC044")]
    public void Plugin_Receives_Correct_Instance()
    {
        var plugin = _container!.GetAllInstances<IPlugin>().OfType<PluginThatResolvesService>().FirstOrDefault();
        plugin.ShouldNotBeNull();
        var p = (PluginThatResolvesService)plugin!;
        p.ResolvedInstance.ShouldNotBeNull();
        p.ResolvedInstance!.GetValue().ShouldBe("Resolved");
    }
}

// helper types
public interface IServiceForResolve { string GetValue(); }
public class ServiceForResolve : IServiceForResolve { public string GetValue() => "Resolved"; }

[PluginId("e1111111-1111-4111-8111-111111111111")]
public class PluginThatResolvesService : Plugin
{
    public IServiceForResolve? ResolvedInstance { get; private set; }

    public override void Install(ServiceRegistry services)
    {
        services.For<IServiceForResolve>().Use<ServiceForResolve>();
    }

    public override async Task Configure(IContainer container, object? host = null)
    {
        ResolvedInstance = container.TryGetInstance<IServiceForResolve>();
        await Task.CompletedTask;
    }
}
