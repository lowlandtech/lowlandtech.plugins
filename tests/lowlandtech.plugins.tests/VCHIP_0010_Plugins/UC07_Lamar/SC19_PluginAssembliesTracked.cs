using Lamar;
using LowlandTech.Plugins.Tests.Fixtures;
using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;
using System.Linq;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC07_Lamar;

[Scenario(
    specId: "VCHIP-0010-UC07-SC19",
    title: "Plugin assemblies are tracked in Lamar",
    given: "Given a plugin is loaded from an external assembly",
    when: "When the plugin is registered in the ServiceRegistry",
    then: "Then the plugin Assemblies collection should contain the loaded assembly")]
public sealed class SC19_PluginAssembliesTracked : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private ServiceRegistry? _services;
    private TestLifecyclePluginLamar? _plugin;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceRegistry();
        _plugin = new TestLifecyclePluginLamar();
    }

    protected override void When()
    {
        _services!.AddPlugin(_plugin!);
    }

    [Fact]
    [Then("the plugin Assemblies collection should contain the loaded assembly", "UAC048")]
    public void Assemblies_Tracked()
    {
        _plugin!.Assemblies.ShouldNotBeNull();
        _plugin!.Assemblies.ShouldNotBeEmpty();
        _plugin!.Assemblies.Any(a => a.GetName().Name == typeof(TestLifecyclePluginLamar).Assembly.GetName().Name).ShouldBeTrue();
    }
}
