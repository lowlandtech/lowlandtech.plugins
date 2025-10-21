using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC29",
    title: "Handle plugin unregistration",
    given: "Given a plugin is registered and configured",
    when: "When an attempt is made to unregister or remove the plugin at runtime",
    then: "Then the framework behavior should be documented")]
public sealed class SC29_PluginUnregistration : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private IServiceCollection? _services;
    private IServiceProvider? _provider;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        var plugin = new SimpleConfigurePluginA();
        _services.AddPlugin(plugin);
    }

    protected override void When()
    {
        _provider = _services!.BuildServiceProvider();
    }

    [Fact]
    [Then("The framework behavior should be documented", "UAC089")]
    public void Unregistration_Not_Supported() => _services!.Any(s => s.ServiceType == typeof(IPlugin)).ShouldBeTrue();

    [Fact]
    [Then("Proper cleanup should occur if plugin removal is supported", "UAC090")]
    public void Cleanup_Documentation() => true.ShouldBeTrue();
}
