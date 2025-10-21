namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

[Scenario(
    specId: "VCHIP-0010-UC04-SC08",
    title: "UsePlugins executes Configure on all loaded plugins",
    given: "Given multiple plugins are registered in the service collection",
    when: "When UsePlugins is called on the container/application",
    then: "Then the Configure method should be called on each plugin")]
public sealed class SC08_UsePluginsCallsConfigure : WhenTestingForV2<LifecycleTestFixture>
{
    private WebApplication? _app;
    private SimpleConfigurePluginA? _p1;
    private SimpleConfigurePluginB? _p2;
    private SimpleConfigurePluginC? _p3;

    protected override LifecycleTestFixture For() => new();

    protected override void Given()
    {
        var builder = WebApplication.CreateBuilder();
        
        // Use distinct types to avoid duplicate plugin-id exception
        _p1 = new SimpleConfigurePluginA();
        _p2 = new SimpleConfigurePluginB();
        _p3 = new SimpleConfigurePluginC();

        builder.Services.AddPlugin(_p1);
        builder.Services.AddPlugin(_p2);
        builder.Services.AddPlugin(_p3);
        
        _app = builder.Build();
    }

    protected override void When()
    {
        // Call UsePlugins to trigger Configure on all registered plugins
        _app!.UsePlugins(host: _app);
    }

    [Fact]
    [Then("The Configure method should be called on each plugin", "UAC025")]
    public void Configure_Called_On_Each()
    {
        _p1!.ConfigureCalled.ShouldBeTrue();
        _p2!.ConfigureCalled.ShouldBeTrue();
        _p3!.ConfigureCalled.ShouldBeTrue();
    }

    [Fact]
    [Then("each plugin should receive the service provider", "UAC026")]
    public void Each_Receives_ServiceProvider()
    {
        _p1!.ReceivedProvider.ShouldNotBeNull();
        _p2!.ReceivedProvider.ShouldNotBeNull();
        _p3!.ReceivedProvider.ShouldNotBeNull();
    }

    [Fact]
    [Then("each plugin should receive the optional host object", "UAC027")]
    public void Each_Receives_Host()
    {
        _p1!.ReceivedHost.ShouldNotBeNull();
        _p2!.ReceivedHost.ShouldNotBeNull();
        _p3!.ReceivedHost.ShouldNotBeNull();
    }

    [Fact]
    [Then("all plugins should complete configuration successfully", "UAC028")]
    public void All_Complete()
    {
        _p1!.ConfigureCalled.ShouldBeTrue();
        _p2!.ConfigureCalled.ShouldBeTrue();
        _p3!.ConfigureCalled.ShouldBeTrue();
    }
}
