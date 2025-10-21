using Lamar;
using LowlandTech.Plugins.Tests.Fixtures;
using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;
using Xunit;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC07_Lamar;

[Scenario(
    specId: "VCHIP-0010-UC07-SC09",
    title: "Lamar ServiceRegistry extension methods are available",
    given: "Given I have a ServiceRegistry instance",
    when: "",
    then: "Then AddPlugins() and AddPlugin(IPlugin) and UsePlugin<T>(host) should be available as extension methods")]
public sealed class SC09_ServiceRegistryExtensionsAvailable : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private ServiceRegistry? _registry;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        // Arrange the ServiceRegistry here so the fact only performs the API-surface checks
        _registry = new ServiceRegistry();
    }

    [Fact]
    [Then("AddPlugins(), AddPlugin(IPlugin) and UsePlugin<T>(host) should be available as extension methods", "UAC022-UAC024")]
    public void ServiceRegistry_Extensions_Available()
    {
        // Single compile-time/API-surface check: invoke extensions to ensure signatures exist
        _registry!.AddPlugins();
        _registry!.AddPlugin(new TestLifecyclePluginLamar());
        _registry!.UsePlugin<TestLifecyclePluginLamar>(host: null);
    }
}
