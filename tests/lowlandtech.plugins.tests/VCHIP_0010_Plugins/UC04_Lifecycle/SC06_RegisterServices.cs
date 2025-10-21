namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

[Scenario(
    specId: "VCHIP-0010-UC04-SC06",
    title: "Plugin registers services during Install",
    given: "Given a plugin needs to register custom services",
    when: "When the Install method is called with the service collection",
    then: "Then the plugin should add singleton services to the collection")]
public sealed class SC06_RegisterServices : WhenTestingForV2<LifecycleTestFixture>
{
    private IServiceCollection? _services;
    private ServiceRegistrationPlugin? _plugin;
    private IServiceProvider? _provider;

    protected override LifecycleTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _plugin = new ServiceRegistrationPlugin();
    }

    protected override void When()
    {
        _services!.AddPlugin(_plugin!);
        _provider = _services.BuildServiceProvider();
    }

    [Fact]
    [Then("The plugin should add singleton services to the collection", "UAC018")]
    public void Plugin_Should_Add_Singleton_Services()
    {
        var singletonService = _provider!.GetService<SingletonTestService>();
        singletonService.ShouldNotBeNull();
        
        // Verify it's actually a singleton
        var singletonService2 = _provider.GetService<SingletonTestService>();
        ReferenceEquals(singletonService, singletonService2).ShouldBeTrue();
    }

    [Fact]
    [Then("The plugin should add scoped services to the collection", "UAC019")]
    public void Plugin_Should_Add_Scoped_Services()
    {
        using var scope1 = _provider!.CreateScope();
        using var scope2 = _provider.CreateScope();
        
        var scopedService1 = scope1.ServiceProvider.GetService<ScopedTestService>();
        var scopedService2 = scope1.ServiceProvider.GetService<ScopedTestService>();
        var scopedService3 = scope2.ServiceProvider.GetService<ScopedTestService>();
        
        scopedService1.ShouldNotBeNull();
        scopedService2.ShouldNotBeNull();
        scopedService3.ShouldNotBeNull();
        
        // Same instance within same scope
        ReferenceEquals(scopedService1, scopedService2).ShouldBeTrue();
        
        // Different instance in different scope
        ReferenceEquals(scopedService1, scopedService3).ShouldBeFalse();
    }

    [Fact]
    [Then("The plugin should add transient services to the collection", "UAC020")]
    public void Plugin_Should_Add_Transient_Services()
    {
        var transientService1 = _provider!.GetService<TransientTestService>();
        var transientService2 = _provider.GetService<TransientTestService>();
        
        transientService1.ShouldNotBeNull();
        transientService2.ShouldNotBeNull();
        
        // Different instances every time
        ReferenceEquals(transientService1, transientService2).ShouldBeFalse();
    }

    [Fact]
    [Then("All registered services should be resolvable after the container is built", "UAC021")]
    public void All_Services_Should_Be_Resolvable()
    {
        _provider!.GetService<SingletonTestService>().ShouldNotBeNull();
        
        using var scope = _provider.CreateScope();
        scope.ServiceProvider.GetService<ScopedTestService>().ShouldNotBeNull();
        
        _provider.GetService<TransientTestService>().ShouldNotBeNull();
    }
}
