namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC02_Validation;

[Scenario(
    specId: "VCHIP-0010-UC02-SC16",
    title: "Validate plugin implements IPlugin interface",
    given: "Given a custom plugin class inherits from Plugin base class",
    when: "When the plugin is instantiated",
    then: "Then typeof(IPlugin).IsAssignableFrom(pluginType) should return true and the plugin should be eligible for registration")]
public class SC16_ValidatePluginImplementsIPlugin : WhenTestingForV2<ValidationTestFixture>
{
    protected override ValidationTestFixture For() => new();

    protected override void Given() { }

    protected override void When() { }

    [Fact]
    [Then("typeof(IPlugin).IsAssignableFrom(pluginType) should return true", "UAC040")]
    public void Type_Should_Implement_IPlugin()
    {
        var pluginType = typeof(ValidPlugin);
        typeof(IPlugin).IsAssignableFrom(pluginType).ShouldBeTrue();
    }

    [Fact]
    [Then("The plugin should be eligible for registration", "UAC041")]
    public void Plugin_Should_Be_Eligible_For_Registration()
    {
        var id = Ardalis.GuardClauses.Guard.Against.MissingPluginId(new ValidPlugin(), nameof(ValidPlugin));
        id.ShouldNotBeNullOrEmpty();
    }
}
