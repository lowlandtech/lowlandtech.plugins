using Lamar;
using LowlandTech.Plugins.Tests.Fixtures;
using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC07_Lamar;

public interface ISingletonTestService
{
    Guid InstanceId { get; }
}

public class SingletonTestService : ISingletonTestService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
}

[PluginId("d1111111-1111-4111-8111-111111111111")]
public class SingletonServicePlugin : Plugin
{
    public override void Install(ServiceRegistry services)
    {
        services.For<ISingletonTestService>().Use<SingletonTestService>().Singleton();
    }
}

[Scenario(
    specId: "VCHIP-0010-UC07-SC14",
    title: "Plugin with Lamar singleton registration",
    given: "Given a plugin registers a singleton service",
    when: "When the service is resolved multiple times",
    then: "Then the same instance should be returned each time")]
public sealed class SC14_PluginWithLamarSingleton : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private ServiceRegistry? _services;
    private IContainer? _container;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceRegistry();
        var plugin = new SingletonServicePlugin();
        _services.AddPlugin(plugin);
    }

    protected override void When()
    {
        _container = new Container(_services);
    }

    [Fact]
    [Then("the same instance should be returned each time", "UAC036")]
    public void Same_Instance_Returned()
    {
        var instance1 = _container!.GetInstance<ISingletonTestService>();
        var instance2 = _container.GetInstance<ISingletonTestService>();
        var instance3 = _container.GetInstance<ISingletonTestService>();
        
        instance1.InstanceId.ShouldBe(instance2.InstanceId);
        instance2.InstanceId.ShouldBe(instance3.InstanceId);
    }

    [Fact]
    [Then("the singleton should live for the container lifetime", "UAC037")]
    public void Singleton_Lives_For_Container_Lifetime()
    {
        var instance1 = _container!.GetInstance<ISingletonTestService>();
        var instanceId = instance1.InstanceId;
        
        // Even in nested scopes, should get the same singleton
        using (var scope = _container.GetNestedContainer())
        {
            var instance2 = scope.GetInstance<ISingletonTestService>();
            instance2.InstanceId.ShouldBe(instanceId);
        }
        
        // After scope disposal, container still has the same singleton
        var instance3 = _container.GetInstance<ISingletonTestService>();
        instance3.InstanceId.ShouldBe(instanceId);
    }
}
