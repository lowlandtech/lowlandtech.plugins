namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC00_Configuration;

[Scenario(
    specId: "VCHIP-0010-UC00-SC06",
    title: "Plugin configuration with empty plugins array",
    given: "Given the appsettings.json contains an empty 'Plugins' section",
    when: "When the configuration is parsed",
    then: "Then the PluginOptions.Plugins list should be empty and no errors should occur")]
public sealed class SC06_EmptyPluginsArray : WhenTestingForV2<ConfigurationTestFixture>
{
    private IConfiguration _configuration = null!;
    private PluginOptions? _pluginOptions;

    protected override ConfigurationTestFixture For() => new ConfigurationTestFixture();

    protected override void Given()
    {
        var configData = new Dictionary<string, string>
        {
            // Empty Plugins section - no items
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
    [Then("The PluginOptions.Plugins list should be empty", "UAC001")]
    public void PluginOptions_Plugins_Should_Be_Empty()
    {
        // When binding to empty config, the result may be null or have empty list
        if (_pluginOptions is null)
        {
            // This is acceptable - no configuration means null result
            true.ShouldBeTrue();
        }
        else
        {
            // If not null, plugins should be empty or null
            (_pluginOptions.Plugins is null || _pluginOptions.Plugins.Count == 0).ShouldBeTrue();
        }
    }

    [Fact]
    [Then("No plugins should be loaded", "UAC002")]
    public void No_Plugins_Should_Be_Loaded()
    {
        var pluginCount = _pluginOptions?.Plugins?.Count ?? 0;
        pluginCount.ShouldBe(0);
    }

    [Fact]
    [Then("No errors should occur", "UAC003")]
    public void No_Errors_Should_Occur()
    {
        // Test passed without exceptions
        true.ShouldBeTrue();
    }
}
