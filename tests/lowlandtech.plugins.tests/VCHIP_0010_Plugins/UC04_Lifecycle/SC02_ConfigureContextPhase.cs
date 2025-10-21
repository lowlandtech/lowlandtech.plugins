using LowlandTech.Plugins.Tests.Fixtures;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

[Scenario(
    specId: "VCHIP-0010-UC04-SC02",
    title: "Execute ConfigureContext phase asynchronously",
    given: "Given a plugin has been installed and the plugin implements ConfigureContext method",
    when: "When the ConfigureContext method is called with the service collection",
    then: "Then the plugin should perform asynchronous context configuration")]
public sealed class SC02_ConfigureContextPhase : WhenTestingForV2<LifecycleTestFixture>
{
    private IServiceCollection? _services;
    private TestLifecyclePlugin? _plugin;
    private Task? _configureContextTask;

    protected override LifecycleTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _plugin = new TestLifecyclePlugin();
        _services.AddPlugin(_plugin);
    }

    protected override void When()
    {
        _configureContextTask = _plugin!.ConfigureContext(_services!);
    }

    [Fact]
    [Then("The plugin should perform asynchronous context configuration", "UAC004")]
    public async Task Plugin_Should_Perform_Async_Configuration()
    {
        await _configureContextTask!;
        _plugin!.ConfigureContextCalled.ShouldBeTrue();
    }

    [Fact]
    [Then("The ConfigureContext method should complete without errors", "UAC005")]
    public async Task ConfigureContext_Should_Complete_Without_Errors()
    {
        await _configureContextTask!;
        _plugin!.ConfigureContextException.ShouldBeNull();
    }

    [Fact]
    [Then("The method should return a completed Task", "UAC006")]
    public async Task Method_Should_Return_Completed_Task()
    {
        await _configureContextTask!;
        _configureContextTask.IsCompleted.ShouldBeTrue();
        _configureContextTask.IsFaulted.ShouldBeFalse();
    }
}
