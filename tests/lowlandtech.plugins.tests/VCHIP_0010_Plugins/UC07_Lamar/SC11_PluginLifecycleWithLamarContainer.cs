using Lamar;
using LowlandTech.Plugins.Tests.Fixtures;
using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC07_Lamar;

[Scenario(
    specId: "VCHIP-0010-UC07-SC11",
    title: "Plugin lifecycle with Lamar Container",
    given: "Given a plugin is registered in ServiceRegistry",
    when: "When the Container is built and UsePlugins is called",
    then: "Then the plugin Install should have been called during registration")]
public sealed class SC11_PluginLifecycleWithLamarContainer : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private ServiceRegistry? _services;
    private IContainer? _container;
    private TestLifecyclePluginLamar? _plugin;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceRegistry();
        _plugin = new TestLifecyclePluginLamar();
        _services.AddPlugin(_plugin);
    }

    protected override void When()
    {
        _container = new Container(_services);
        _container.UsePlugins().GetAwaiter().GetResult();
    }

    [Fact]
    [Then("the plugin Install should have been called during registration", "UAC027")]
    public void Install_Called_During_Registration()
    {
        _plugin!.InstallCalled.ShouldBeTrue();
    }

    [Fact]
    [Then("the plugin Configure should be called when UsePlugins executes", "UAC028")]
    public void Configure_Called_When_UsePlugins()
    {
        _plugin!.ConfigureCalled.ShouldBeTrue();
    }

    [Fact]
    [Then("the plugin should have full access to Lamar container features", "UAC029")]
    public void Plugin_Has_Container_Access()
    {
        _plugin!.ServiceProviderReceived.ShouldNotBeNull();
        // Verify it's actually a Lamar container
        _plugin.ServiceProviderReceived.ShouldBeOfType<Container>();
    }
}
