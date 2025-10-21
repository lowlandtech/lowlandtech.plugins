using Lamar;
using LowlandTech.Plugins.Tests.Fixtures;
using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC07_Lamar;

[Scenario(
    specId: "VCHIP-0010-UC07-SC16",
    title: "Lamar integration without ASP.NET Core",
    given: "Given I'm building a console application with Lamar",
    when: "When I use the core LowlandTech.Plugins package",
    then: "Then I should be able to register and use plugins")]
public sealed class SC16_LamarIntegrationWithoutAspNetCore : WhenTestingForV2<ErrorHandlingTestFixture>
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
        // Build Lamar container and run plugin lifecycle without ASP.NET Core
        _container = new Container(_services);
        _container.UsePlugins().GetAwaiter().GetResult();
    }

    [Fact]
    [Then("Then I should be able to register and use plugins", "UAC040")]
    public void Plugins_Usable_Without_AspNetCore()
    {
        _container.ShouldNotBeNull();
        _plugin!.ConfigureCalled.ShouldBeTrue();
    }
}
