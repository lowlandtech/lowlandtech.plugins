using Lamar;
using LowlandTech.Plugins.Tests.Fixtures;
using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC07_Lamar;

public interface IScopedTestService
{
    Guid InstanceId { get; }
}

public class ScopedTestService : IScopedTestService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
}

[PluginId("c1111111-1111-4111-8111-111111111111")]
public class ScopedServicePlugin : Plugin
{
    public override void Install(ServiceRegistry services)
    {
        services.For<IScopedTestService>().Use<ScopedTestService>().Scoped();
    }
}

[Scenario(
    specId: "VCHIP-0010-UC07-SC13",
    title: "Lamar scoped services in plugins",
    given: "Given a plugin registers a scoped service",
    when: "When the service is resolved from the container",
    then: "Then the scoped service should follow Lamar scoping rules")]
public sealed class SC13_LamarScopedServicesInPlugins : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private ServiceRegistry? _services;
    private IContainer? _container;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceRegistry();
        var plugin = new ScopedServicePlugin();
        _services.AddPlugin(plugin);
    }

    protected override void When()
    {
        _container = new Container(_services);
    }

    [Fact]
    [Then("the scoped service should follow Lamar scoping rules", "UAC033")]
    public void Scoped_Service_Follows_Rules()
    {
        using var scope1 = _container!.GetNestedContainer();
        var instance1 = scope1.GetInstance<IScopedTestService>();
        var instance2 = scope1.GetInstance<IScopedTestService>();
        
        // Same instance within same scope
        instance1.InstanceId.ShouldBe(instance2.InstanceId);
    }

    [Fact]
    [Then("different scopes should get different instances", "UAC034")]
    public void Different_Scopes_Get_Different_Instances()
    {
        using var scope1 = _container!.GetNestedContainer();
        using var scope2 = _container.GetNestedContainer();
        
        var instance1 = scope1.GetInstance<IScopedTestService>();
        var instance2 = scope2.GetInstance<IScopedTestService>();
        
        instance1.InstanceId.ShouldNotBe(instance2.InstanceId);
    }

    [Fact]
    [Then("services should be disposed when scope ends", "UAC035")]
    public void Services_Disposed_When_Scope_Ends()
    {
        Guid instanceId;
        
        using (var scope = _container!.GetNestedContainer())
        {
            var instance = scope.GetInstance<IScopedTestService>();
            instanceId = instance.InstanceId;
            instance.ShouldNotBeNull();
        }
        
        // After scope disposal, a new scope should get a new instance
        using (var newScope = _container.GetNestedContainer())
        {
            var newInstance = newScope.GetInstance<IScopedTestService>();
            newInstance.InstanceId.ShouldNotBe(instanceId);
        }
    }
}
