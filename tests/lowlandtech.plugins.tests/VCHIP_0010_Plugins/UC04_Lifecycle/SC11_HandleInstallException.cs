namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

[Scenario(
    specId: "VCHIP-0010-UC04-SC11",
    title: "Handle exception during Install phase",
    given: "Given a plugin throws an exception during Install",
    when: "When the plugin registration process attempts to call Install",
    then: "Then the exception should be propagated")]
public sealed class SC11_HandleInstallException : WhenTestingForV2<LifecycleTestFixture>
{
    private IServiceCollection? _services;
    private ThrowingInstallPlugin? _plugin;
    private Exception? _caught;

    protected override LifecycleTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _plugin = new ThrowingInstallPlugin();
    }

    protected override void When()
    {
        try
        {
            _services!.AddPlugin(_plugin!);
        }
        catch (Exception ex)
        {
            _caught = ex;
        }
    }

    [Fact]
    [Then("The exception should be propagated", "UAC035")]
    public void Exception_Propagated()
    {
        _caught.ShouldNotBeNull();
        _caught.ShouldBeOfType<InvalidOperationException>();
    }

    [Fact]
    [Then("The plugin registration should fail", "UAC036")]
    public void Registration_Fails()
    {
        var desc = _services!.FirstOrDefault(d => d.ServiceType == typeof(IPlugin));
        // If registration failed, desc should be null or not contain an instance of the throwing plugin
        if (desc is not null && desc.ImplementationInstance is IPlugin pi)
        {
            pi.GetType().ShouldNotBe(typeof(ThrowingInstallPlugin));
        }
    }

    [Fact]
    [Then("Subsequent plugins should not be affected", "UAC037")]
    public void Subsequent_Not_Affected()
    {
        // Register another plugin and ensure it works
        var ok = new SimpleConfigurePlugin();
        _services!.AddPlugin(ok);
        var sp = _services.BuildServiceProvider();
        var loaded = sp.GetServices<IPlugin>().Any(p => p.GetType() == typeof(SimpleConfigurePlugin));
        loaded.ShouldBeTrue();
    }
}
