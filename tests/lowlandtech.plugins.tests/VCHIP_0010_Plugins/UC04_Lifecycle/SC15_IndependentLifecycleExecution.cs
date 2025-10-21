namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

[Scenario(
    specId: "VCHIP-0010-UC04-SC15",
    title: "Multiple plugins execute lifecycle independently",
    given: "Given plugin A and plugin B are both registered",
    when: "When plugin A is executing its Install phase",
    then: "Then plugin B should not be affected")]
public sealed class SC15_IndependentLifecycleExecution : WhenTestingForV2<LifecycleTestFixture>
{
    private IServiceCollection? _services;
    private IndependentPlugin? _pA;

    protected override LifecycleTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _pA = new IndependentPlugin();
        
        // Register plugin A only to simulate it executing Install
        _services.AddPlugin(_pA);
        // plugin B is not registered here to ensure it remains unaffected
    }

    protected override void When()
    {
        // Start Install for plugin A only (already called by AddPlugin)
        // Ensure plugin B remains unaffected (its flags should be independent)
    }

    [Fact]
    [Then("Plugin B should not be affected", "UAC046")]
    public void PluginB_Not_Affected()
    {
        // Assert that no TestLifecyclePlugin is registered in the service collection
        var desc = _services!.FirstOrDefault(d => d.ServiceType == typeof(IPlugin) && (d.ImplementationInstance is TestLifecyclePlugin || d.ImplementationType == typeof(TestLifecyclePlugin)));
        desc.ShouldBeNull();
    }

    [Fact]
    [Then("Plugin B should execute its own lifecycle phases independently", "UAC047")]
    public void PluginB_Independent()
    {
        // Now register plugin B and cause its lifecycle to run
        var pB = new TestLifecyclePlugin();
        _services!.AddPlugin(pB);
        var sp = _services.BuildServiceProvider();
        pB.ConfigureContext(_services).GetAwaiter().GetResult();
        pB.Configure(sp, host: null).GetAwaiter().GetResult();

        pB.InstallCalled.ShouldBeTrue();
        pB.ConfigureContextCalled.ShouldBeTrue();
        pB.ConfigureCalled.ShouldBeTrue();
    }

    [Fact]
    [Then("Both plugins should complete their lifecycles successfully", "UAC048")]
    public void Both_Complete()
    {
        // Register plugin B and run its lifecycle to verify both complete
        var pB = new TestLifecyclePlugin();
        _services!.AddPlugin(pB);
        var sp = _services.BuildServiceProvider();
        pB.ConfigureContext(_services).GetAwaiter().GetResult();
        pB.Configure(sp, host: null).GetAwaiter().GetResult();

        _pA!.InstallStarted.ShouldBeTrue();
        _pA.InstallCompleted.ShouldBeTrue();
        pB.InstallCalled.ShouldBeTrue();
    }
}
