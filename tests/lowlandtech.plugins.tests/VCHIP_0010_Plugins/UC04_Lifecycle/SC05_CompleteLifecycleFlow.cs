namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

[Scenario(
    specId: "VCHIP-0010-UC04-SC05",
    title: "Complete lifecycle flow - Install, ConfigureContext, Configure",
    given: "Given a new plugin is being added to the application",
    when: "When the complete lifecycle executes in order",
    then: "Then the Install phase should execute first")]
public sealed class SC05_CompleteLifecycleFlow : WhenTestingForV2<LifecycleTestFixture>
{
    private IServiceCollection? _services;
    private TestLifecyclePlugin? _plugin;
    private IServiceProvider? _provider;
    private readonly List<string> _executionOrder = new();
    private bool _lifecycleCompleted = false;

    protected override LifecycleTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _plugin = new TestLifecyclePlugin();
        _plugin.OnPhaseExecuted = (phase) => _executionOrder.Add(phase);
    }

    protected override void When()
    {
        // Execute the complete lifecycle synchronously
        // Phase 1: Install
        _services!.AddPlugin(_plugin!);
        _provider = _services.BuildServiceProvider();
        
        // Phase 2: ConfigureContext
        _plugin!.ConfigureContext(_services).GetAwaiter().GetResult();
        
        // Phase 3: Configure
        _plugin.Configure(_provider!, host: null).GetAwaiter().GetResult();
        
        _lifecycleCompleted = true;
    }

    [Fact]
    [Then("The Install phase should execute first", "UAC014")]
    public void Install_Should_Execute_First()
    {
        _lifecycleCompleted.ShouldBeTrue();
        _executionOrder.ShouldNotBeEmpty();
        _executionOrder[0].ShouldBe("Install");
    }

    [Fact]
    [Then("The ConfigureContext phase should execute after Install", "UAC015")]
    public void ConfigureContext_Should_Execute_After_Install()
    {
        _lifecycleCompleted.ShouldBeTrue();
        _executionOrder.Count.ShouldBeGreaterThanOrEqualTo(2);
        _executionOrder[1].ShouldBe("ConfigureContext");
    }

    [Fact]
    [Then("The Configure phase should execute last", "UAC016")]
    public void Configure_Should_Execute_Last()
    {
        _lifecycleCompleted.ShouldBeTrue();
        _executionOrder.Count.ShouldBe(3);
        _executionOrder[2].ShouldBe("Configure");
    }

    [Fact]
    [Then("All phases should complete successfully", "UAC017")]
    public void All_Phases_Should_Complete_Successfully()
    {
        _lifecycleCompleted.ShouldBeTrue();
        _plugin!.InstallCalled.ShouldBeTrue();
        _plugin.ConfigureContextCalled.ShouldBeTrue();
        _plugin.ConfigureCalled.ShouldBeTrue();
        
        _plugin.InstallException.ShouldBeNull();
        _plugin.ConfigureContextException.ShouldBeNull();
        _plugin.ConfigureException.ShouldBeNull();
    }
}
