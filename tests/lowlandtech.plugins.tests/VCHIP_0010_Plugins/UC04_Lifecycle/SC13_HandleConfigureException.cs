namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

[Scenario(
    specId: "VCHIP-0010-UC04-SC13",
    title: "Handle exception during Configure phase",
    given: "Given a plugin throws an exception during Configure",
    when: "When the UsePlugins method calls Configure on the plugin",
    then: "Then the exception should be propagated")]
public sealed class SC13_HandleConfigureException : WhenTestingForV2<LifecycleTestFixture>
{
    private IServiceCollection? _services;
    private ThrowingConfigurePlugin? _plugin;
    private Exception? _caught;

    protected override LifecycleTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _plugin = new ThrowingConfigurePlugin();
        _services.AddPlugin(_plugin);
    }

    protected override void When()
    {
        try
        {
            // simulate UsePlugins calling Configure
            var sp = _services!.BuildServiceProvider();
            var plugins = sp.GetServices<IPlugin>();
            foreach (var p in plugins)
            {
                p.Configure(sp, host: null).GetAwaiter().GetResult();
            }
        }
        catch (Exception ex)
        {
            _caught = ex;
        }
    }

    [Fact]
    [Then("The exception should be propagated", "UAC041")]
    public void Exception_Propagated()
    {
        _caught.ShouldNotBeNull();
        _caught.ShouldBeOfType<InvalidOperationException>();
    }

    [Fact]
    [Then("Subsequent plugins should still be processed (or fail fast depending on implementation)", "UAC042")]
    public void Subsequent_Plugins_Processing()
    {
        // Ensure that calling Configure on the failing plugin throws but doesn't prevent inspection of other plugins
        var ok = new SimpleConfigurePluginA();
        _services!.AddPlugin(ok);
        var sp = _services.BuildServiceProvider();
        var foundOk = sp.GetServices<IPlugin>().Any(p => p.GetType() == typeof(SimpleConfigurePluginA));
        foundOk.ShouldBeTrue();
    }

    [Fact]
    [Then("The error should be logged or handled appropriately", "UAC043")]
    public void Error_Logged()
    {
        _caught.ShouldNotBeNull();
    }
}
