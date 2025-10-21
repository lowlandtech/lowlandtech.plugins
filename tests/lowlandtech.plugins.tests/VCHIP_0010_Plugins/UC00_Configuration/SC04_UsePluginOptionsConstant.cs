namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC00_Configuration;

[Scenario(
    specId: "VCHIP-0010-UC00-SC04",
    title: "Use PluginOptions constant for configuration section name",
    given: "Given I need to bind plugin configuration from appsettings",
    when: "When I use the PluginOptions.Name constant",
    then: "Then the constant should equal 'Plugins' and configuration should bind correctly")]
public sealed class SC04_UsePluginOptionsConstant : WhenTestingForV2<ConfigurationTestFixture>
{
    private IConfiguration _configuration = null!;
    private PluginOptions? _pluginOptions;

    protected override ConfigurationTestFixture For() => new ConfigurationTestFixture();

    protected override void Given()
    {
        var configData = new Dictionary<string, string>
        {
            ["Plugins:Plugins:0:Name"] = "LowlandTech.Sample.Backend",
            ["Plugins:Plugins:0:IsActive"] = "true"
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();
    }

    protected override void When()
    {
        // Use the constant to retrieve configuration
        _pluginOptions = _configuration
            .GetSection(PluginOptions.Name)
            .Get<PluginOptions>();
    }

    [Fact]
    [Then("The constant should equal 'Plugins'", "UAC001")]
    public void PluginOptions_Name_Constant_Should_Be_Plugins()
    {
        PluginOptions.Name.ShouldBe("Plugins");
    }

    [Fact]
    [Then("The configuration should bind correctly using this constant", "UAC002")]
    public void Configuration_Should_Bind_Using_Constant()
    {
        _pluginOptions.ShouldNotBeNull();
        _pluginOptions.Plugins.ShouldNotBeNull();
        _pluginOptions.Plugins.Count.ShouldBe(1);
        _pluginOptions.Plugins[0].Name.ShouldBe("LowlandTech.Sample.Backend");
    }
}
