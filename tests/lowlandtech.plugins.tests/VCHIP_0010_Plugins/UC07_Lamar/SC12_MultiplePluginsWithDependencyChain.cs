using Lamar;
using LowlandTech.Plugins.Tests.Fixtures;
using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC07_Lamar;

// Define test plugins for dependency chain testing
[PluginId("b1111111-1111-4111-8111-111111111111")]
public class PluginA : Plugin
{
    public override void Install(ServiceRegistry services)
    {
        services.For<IServiceA>().Use<ServiceA>();
    }
}

[PluginId("b2222222-2222-4222-8222-222222222222")]
public class PluginB : Plugin
{
    public IServiceA? ResolvedServiceA { get; private set; }

    public override void Install(ServiceRegistry services)
    {
        services.For<IServiceB>().Use<ServiceB>();
    }

    public override async Task Configure(IContainer container, object? host = null)
    {
        // Try to resolve ServiceA that PluginA registered
        ResolvedServiceA = container.TryGetInstance<IServiceA>();
        await Task.CompletedTask;
    }
}

public interface IServiceA
{
    string GetValue();
}

public class ServiceA : IServiceA
{
    public string GetValue() => "ServiceA";
}

public interface IServiceB
{
    string GetValue();
}

public class ServiceB : IServiceB
{
    private readonly IServiceA _serviceA;

    public ServiceB(IServiceA serviceA)
    {
        _serviceA = serviceA;
    }

    public string GetValue() => $"ServiceB with {_serviceA.GetValue()}";
}

[Scenario(
    specId: "VCHIP-0010-UC07-SC12",
    title: "Multiple plugins with Lamar dependency chain",
    given: "Given Plugin A registers ServiceA and Plugin B depends on ServiceA",
    when: "When the container is built and UsePlugins is called",
    then: "Then Plugin A services should be available to Plugin B")]
public sealed class SC12_MultiplPluginsWithDependencyChain : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private ServiceRegistry? _services;
    private IContainer? _container;
    private PluginA? _pluginA;
    private PluginB? _pluginB;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceRegistry();
        _pluginA = new PluginA();
        _pluginB = new PluginB();
        
        _services.AddPlugin(_pluginA);
        _services.AddPlugin(_pluginB);
    }

    protected override void When()
    {
        _container = new Container(_services);
        _container.UsePlugins().GetAwaiter().GetResult();
    }

    [Fact]
    [Then("Plugin A services should be available to Plugin B", "UAC030")]
    public void PluginA_Services_Available_To_PluginB()
    {
        _pluginB!.ResolvedServiceA.ShouldNotBeNull();
        _pluginB.ResolvedServiceA!.GetValue().ShouldBe("ServiceA");
    }

    [Fact]
    [Then("the dependency chain should resolve correctly", "UAC031")]
    public void Dependency_Chain_Resolves()
    {
        var serviceB = _container!.TryGetInstance<IServiceB>();
        serviceB.ShouldNotBeNull();
        serviceB.GetValue().ShouldBe("ServiceB with ServiceA");
    }

    [Fact]
    [Then("no circular dependencies should occur", "UAC032")]
    public void No_Circular_Dependencies()
    {
        // If we got here without exceptions, there are no circular dependencies
        _container.ShouldNotBeNull();
        var serviceA = _container!.GetInstance<IServiceA>();
        var serviceB = _container.GetInstance<IServiceB>();
        serviceA.ShouldNotBeNull();
        serviceB.ShouldNotBeNull();
    }
}
