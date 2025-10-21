using Lamar;
using LowlandTech.Plugins.Tests.Fixtures;
using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC07_Lamar;

[Scenario(
    specId: "VCHIP-0010-UC07-SC05",
    title: "UsePlugins with custom host object",
    given: "Given plugins are registered in a Lamar container",
    when: "When I call container.UsePlugins(customHost)",
    then: "Then each plugin Configure method should receive the custom host")]
public sealed class SC05_UsePluginsWithCustomHost : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private ServiceRegistry? _services;
    private IContainer? _container;
    private TestLifecyclePluginLamar? _plugin;
    private object? _host;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceRegistry();
        _plugin = new TestLifecyclePluginLamar();
        _services.AddPlugin(_plugin);
        _container = new Container(_services);
    }

    protected override void When()
    {
        _host = new { Name = "CustomHost" };
        _container!.UsePlugins(_host).GetAwaiter().GetResult();
    }

    [Fact]
    [Then("each plugin Configure method should receive the custom host", "UAC013")]
    public void Configure_Receives_Host() => _plugin!.HostReceived.ShouldNotBeNull();

    [Fact]
    [Then("plugins should be able to configure themselves with the host", "UAC014")]
    public void Plugins_Configure_With_Host() => _plugin!.ConfigureCalled.ShouldBeTrue();
}
