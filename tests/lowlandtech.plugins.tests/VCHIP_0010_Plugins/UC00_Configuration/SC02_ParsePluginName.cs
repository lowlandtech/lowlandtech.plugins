namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC00_Configuration;

[Scenario(
    specId: "VCHIP-0010-UC00-SC02",
    title: "Parse plugin name from configuration",
    given: "Given the appsettings.json contains plugin configuration with Name property",
    when: "When the configuration is parsed into PluginConfig objects",
    then: "Then the plugin name should be correctly extracted without .dll extension")]
public sealed class SC02_ParsePluginName : WhenTestingForV2<ConfigurationTestFixture>
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
        _pluginOptions = _configuration
            .GetSection(PluginOptions.Name)
            .Get<PluginOptions>();
    }

    [Fact]
    [Then("The plugin name should be 'LowlandTech.Sample.Backend'", "UAC001")]
    public void Plugin_Name_Should_Match_Configuration()
    {
        _pluginOptions.ShouldNotBeNull();
        _pluginOptions.Plugins.ShouldNotBeNull();
        _pluginOptions.Plugins.Count.ShouldBe(1);
        _pluginOptions.Plugins[0].Name.ShouldBe("LowlandTech.Sample.Backend");
    }

    [Fact]
    [Then("The name should not include the .dll extension", "UAC002")]
    public void Plugin_Name_Should_Not_Include_Dll_Extension()
    {
        _pluginOptions.ShouldNotBeNull();
        _pluginOptions.Plugins.ShouldNotBeNull();
        _pluginOptions.Plugins[0].Name.ShouldNotEndWith(".dll");
    }
}
