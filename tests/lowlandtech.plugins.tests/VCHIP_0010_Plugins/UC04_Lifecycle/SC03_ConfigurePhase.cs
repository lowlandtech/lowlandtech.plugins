namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

[Scenario(
    specId: "VCHIP-0010-UC04-SC03",
    title: "Execute Configure phase with service provider",
    given: "Given a plugin has been installed and the service provider has been built",
    when: "When the Configure method is called with the service provider",
    then: "Then the plugin should have access to all registered services")]
public sealed class SC03_ConfigurePhase : WhenTestingForV2<LifecycleTestFixture>
{
    private IServiceCollection? _services;
    private TestLifecyclePlugin? _plugin;
    private IServiceProvider? _provider;
    private Task? _configureTask;

    protected override LifecycleTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _plugin = new TestLifecyclePlugin();
        _services.AddPlugin(_plugin);
        _provider = _services.BuildServiceProvider();
    }

    protected override void When()
    {
        _configureTask = _plugin!.Configure(_provider!, host: null);
    }

    [Fact]
    [Then("The plugin should have access to all registered services", "UAC007")]
    public async Task Plugin_Should_Have_Access_To_Services()
    {
        await _configureTask!;
        _plugin!.ConfigureCalled.ShouldBeTrue();
        _plugin.ServiceProviderReceived.ShouldNotBeNull();
        
        // Verify plugin can resolve its own registered service
        var testService = _plugin.ServiceProviderReceived!.GetService<TestPluginService>();
        testService.ShouldNotBeNull();
    }

    [Fact]
    [Then("The plugin should complete its configuration", "UAC008")]
    public async Task Plugin_Should_Complete_Configuration()
    {
        await _configureTask!;
        _plugin!.ConfigureCalled.ShouldBeTrue();
        _plugin.ConfigureException.ShouldBeNull();
    }

    [Fact]
    [Then("The Configure method should return a completed Task", "UAC009")]
    public async Task Configure_Should_Return_Completed_Task()
    {
        await _configureTask!;
        _configureTask.IsCompleted.ShouldBeTrue();
        _configureTask.IsFaulted.ShouldBeFalse();
    }
}
