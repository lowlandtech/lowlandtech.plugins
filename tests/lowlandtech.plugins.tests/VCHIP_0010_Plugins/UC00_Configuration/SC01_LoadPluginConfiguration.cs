namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC00_Configuration;

[Scenario(
    specId: "VCHIP-0010-UC00-SC01",
    title: "Load plugin configuration from appsettings.json",
    given: "Given the appsettings.json contains a 'Plugins' section with plugin entries",
    when: "When the configuration is read",
    then: "Then the PluginOptions should be populated and contain all configured plugins")]
public sealed class SC01_LoadPluginConfiguration : WhenTestingForV2<ConfigurationTestFixture>
{
    private IConfiguration _configuration = null!;
    private PluginOptions? _pluginOptions;

    protected override ConfigurationTestFixture For() => new ConfigurationTestFixture();

    protected override void Given()
    {
        // Create in-memory configuration with Plugins section
        var configData = new Dictionary<string, string>
        {
            ["Plugins:Plugins:0:Name"] = "LowlandTech.Sample.Backend",
            ["Plugins:Plugins:0:IsActive"] = "true",
            ["Plugins:Plugins:1:Name"] = "LowlandTech.Sample.Frontend",
            ["Plugins:Plugins:1:IsActive"] = "true"
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
    [Then("The PluginOptions should be populated", "UAC001")]
    public void PluginOptions_Should_Be_Populated()
    {
        _pluginOptions.ShouldNotBeNull();
    }

    [Fact]
    [Then("The PluginOptions.Plugins list should contain all configured plugins", "UAC002")]
    public void PluginOptions_Should_Contain_All_Configured_Plugins()
    {
        _pluginOptions.ShouldNotBeNull();
        _pluginOptions.Plugins.ShouldNotBeNull();
        _pluginOptions.Plugins.Count.ShouldBe(2);

        _pluginOptions.Plugins[0].Name.ShouldBe("LowlandTech.Sample.Backend");
        _pluginOptions.Plugins[0].IsActive.ShouldBeTrue();

        _pluginOptions.Plugins[1].Name.ShouldBe("LowlandTech.Sample.Frontend");
        _pluginOptions.Plugins[1].IsActive.ShouldBeTrue();
    }
}
