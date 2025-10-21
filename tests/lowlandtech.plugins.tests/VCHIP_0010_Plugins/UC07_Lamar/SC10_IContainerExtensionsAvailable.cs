using Lamar;
using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;
using Xunit;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC07_Lamar;

[Scenario(
    specId: "VCHIP-0010-UC07-SC10",
    title: "Lamar IContainer extension methods are available",
    given: "Given I have an IContainer instance",
    when: "",
    then: "Then UsePlugins() should be available as an extension method")]
public sealed class SC10_IContainerExtensionsAvailable : WhenTestingForV2<ErrorHandlingTestFixture>
{
    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given() { }
    protected override void When() { }

    [Fact]
    [Then("UsePlugins() should be available as an extension method", "UAC025")]
    public void UsePlugins_Available()
    {
        var registry = new ServiceRegistry();
        var container = new Container(registry);
        // compile-time check
        container.UsePlugins();
        container.UsePlugins(host: null);
    }

    [Fact]
    [Then("UsePlugins(host) should be available with optional host parameter", "UAC026")]
    public void UsePlugins_Host_Optional()
    {
        var registry = new ServiceRegistry();
        var container = new Container(registry);
        container.UsePlugins(host: new { Name = "Host" });
    }
}
