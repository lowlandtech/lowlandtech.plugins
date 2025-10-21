namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC00_Configuration;

[Scenario(
    specId: "VCHIP-0010-UC00-SC03",
    title: "Parse IsActive flag from configuration",
    given: "Given the appsettings.json contains plugin configuration with IsActive set to false",
    when: "When the configuration is parsed into PluginConfig objects",
    then: "Then the plugin IsActive flag should be false and the plugin should not be loaded")]
public sealed class SC03_ParseIsActiveFlag : WhenTestingForV2<ConfigurationTestFixture>
{
    private IConfiguration _configuration = null!;
    private PluginOptions? _pluginOptions;

    protected override ConfigurationTestFixture For() => new ConfigurationTestFixture();

    protected override void Given()
    {
        var configData = new Dictionary<string, string>
        {
            ["Plugins:Plugins:0:Name"] = "LowlandTech.Sample.Backend",
            ["Plugins:Plugins:0:IsActive"] = "false"
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
    [Then("The plugin IsActive flag should be false", "UAC001")]
    public void Plugin_IsActive_Should_Be_False()
    {
        _pluginOptions.ShouldNotBeNull();
        _pluginOptions.Plugins.ShouldNotBeNull();
        _pluginOptions.Plugins.Count.ShouldBe(1);
        _pluginOptions.Plugins[0].IsActive.ShouldBeFalse();
    }

    [Fact]
    [Then("The plugin should not be loaded", "UAC002")]
    public void Inactive_Plugin_Should_Not_Be_Loaded()
    {
        _pluginOptions.ShouldNotBeNull();

        // Filter for active plugins only (simulating the loading logic)
        var activePlugins = _pluginOptions.Plugins
            .Where(p => p.IsActive)
            .ToList();

        activePlugins.Count.ShouldBe(0);
    }
}
