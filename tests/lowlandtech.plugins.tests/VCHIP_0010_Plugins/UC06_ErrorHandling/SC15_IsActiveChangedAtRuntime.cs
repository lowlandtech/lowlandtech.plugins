
using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC15",
    title: "Handle plugin with IsActive set to false after registration",
    given: "Given a plugin is registered with IsActive = true",
    when: "When the IsActive property is changed to false at runtime",
    then: "Then the plugin behavior depends on implementation")]
public sealed class SC15_IsActiveChangedAtRuntime : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private IServiceCollection? _services;
    private SimpleConfigurePluginA? _p;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _p = new SimpleConfigurePluginA();
        _p.IsActive = true;
        _services.AddPlugin(_p);
    }

    protected override void When() { }

    [Fact]
    [Then("Changing IsActive at runtime may not affect already-configured plugins", "UAC048")]
    public void IsActive_Change()
    {
        // flip the flag after registration
        _p!.IsActive = false;
        // Build provider and ensure plugin still registered because AddPlugin already executed Install
        var sp = _services!.BuildServiceProvider();
        sp.GetServices<IPlugin>().ShouldContain(p => p.GetType() == typeof(SimpleConfigurePluginA));
    }
}
