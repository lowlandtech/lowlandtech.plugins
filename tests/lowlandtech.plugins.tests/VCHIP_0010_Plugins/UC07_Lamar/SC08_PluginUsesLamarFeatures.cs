using Lamar;
using LowlandTech.Plugins.Tests.Fixtures;
using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC07_Lamar;

[Scenario(
    specId: "VCHIP-0010-UC07-SC08",
    title: "Plugin with Lamar-specific features",
    given: "Given a plugin uses Lamar's advanced registration features",
    when: "When the plugin Install method uses ServiceRegistry features like Policies, Interceptors, Decorated instances",
    then: "Then the Lamar-specific features should work correctly")]
public sealed class SC08_PluginUsesLamarFeatures : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private ServiceRegistry? _services;
    private IContainer? _container;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceRegistry();
    }

    protected override void When()
    {
        var plugin = new LamarAdvancedPlugin();
        _services!.AddPlugin(plugin);
        _container = new Container(_services);
    }

    [Fact]
    [Then("the Lamar-specific features should work correctly", "UAC020")]
    public void Lamar_Features_Work()
    {
        // Verify interceptor/policy/decorator behaviour by resolving service
        var svc = _container!.GetInstance<ILamarAdvancedService>();
        svc.ShouldNotBeNull();
        var msg = svc.GetMessage();
        msg.ShouldContain("advanced");
    }

    [Fact]
    [Then("the plugin should integrate seamlessly", "UAC021")]
    public void Plugin_Integrates()
    {
        var svc = _container!.GetInstance<ILamarAdvancedService>();
        svc.GetMessage().ShouldContain("Lamar advanced");
    }
}
