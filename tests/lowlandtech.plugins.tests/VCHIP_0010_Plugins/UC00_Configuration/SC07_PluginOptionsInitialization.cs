namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC00_Configuration;

[Scenario(
    specId: "VCHIP-0010-UC00-SC07",
    title: "PluginOptions initialization with default values",
    given: "Given a new PluginOptions instance is created",
    when: "When no configuration is provided",
    then: "Then the Plugins list should be initialized as an empty list and not be null")]
public sealed class SC07_PluginOptionsInitialization : WhenTestingForV2<ConfigurationTestFixture>
{
    private PluginOptions? _pluginOptions;

    protected override ConfigurationTestFixture For() => new ConfigurationTestFixture();

    protected override void Given()
    {
        // No setup needed
    }

    protected override void When()
    {
        _pluginOptions = new PluginOptions();
    }

    [Fact]
    [Then("The Plugins list should be initialized as an empty list", "UAC001")]
    public void Plugins_List_Should_Be_Initialized()
    {
        _pluginOptions.ShouldNotBeNull();
        _pluginOptions.Plugins.ShouldNotBeNull();
        _pluginOptions.Plugins.Count.ShouldBe(0);
    }

    [Fact]
    [Then("The list should not be null", "UAC002")]
    public void Plugins_List_Should_Not_Be_Null()
    {
        _pluginOptions.ShouldNotBeNull();
        _pluginOptions.Plugins.ShouldNotBeNull();
    }
}
