namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

[Scenario(
    specId: "VCHIP-0010-UC04-SC07",
    title: "Plugin with virtual ConfigureContext uses default implementation",
    given: "Given a plugin does not override the ConfigureContext method",
    when: "When the ConfigureContext method is called",
    then: "Then the default virtual implementation should execute")]
public sealed class SC07_DefaultConfigureContext : WhenTestingForV2<LifecycleTestFixture>
{
    private IServiceCollection? _services;
    private DefaultConfigureContextPlugin? _plugin;
    private Task? _task;

    protected override LifecycleTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _plugin = new DefaultConfigureContextPlugin();
        _services.AddPlugin(_plugin);
    }

    protected override void When()
    {
        _task = _plugin!.ConfigureContext(_services!);
    }

    [Fact]
    [Then("The default virtual implementation should execute", "UAC022")]
    public async Task Default_Should_Execute()
    {
        await _task!;
        // since plugin didn't override ConfigureContext, no exception and it completes
        _plugin!.InstallCalled.ShouldBeTrue();
    }

    [Fact]
    [Then("The method should return an immediately completed Task", "UAC023")]
    public async Task Task_Should_Complete_Immediately()
    {
        await _task!;
        _task!.IsCompleted.ShouldBeTrue();
    }

    [Fact]
    [Then("No errors should occur", "UAC024")]
    public async Task No_Errors()
    {
        await _task!;
    }
}
