namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC00_Configuration;

[Scenario(
    specId: "VCHIP-0010-UC00-SC05",
    title: "Handle multiple plugin configurations",
    given: "Given the appsettings.json contains multiple plugin configurations with varying IsActive flags",
    when: "When the configuration is parsed",
    then: "Then all PluginConfig objects should be created with correct active/inactive counts")]
public sealed class SC05_HandleMultiplePluginConfigurations : WhenTestingForV2<ConfigurationTestFixture>
{
    private IConfiguration _configuration = null!;
    private PluginOptions? _pluginOptions;

    protected override ConfigurationTestFixture For() => new ConfigurationTestFixture();

    protected override void Given()
    {
        var configData = new Dictionary<string, string>
        {
            ["Plugins:Plugins:0:Name"] = "LowlandTech.Sample.Backend",
            ["Plugins:Plugins:0:IsActive"] = "true",
            ["Plugins:Plugins:1:Name"] = "LowlandTech.Sample.Frontend",
            ["Plugins:Plugins:1:IsActive"] = "true",
            ["Plugins:Plugins:2:Name"] = "LowlandTech.Sample.Reporting",
            ["Plugins:Plugins:2:IsActive"] = "false"
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
    [Then("3 PluginConfig objects should be created", "UAC001")]
    public void Should_Create_Three_PluginConfig_Objects()
    {
        _pluginOptions.ShouldNotBeNull();
        _pluginOptions.Plugins.ShouldNotBeNull();
        _pluginOptions.Plugins.Count.ShouldBe(3);
    }

    [Fact]
    [Then("2 plugins should be marked as active", "UAC002")]
    public void Two_Plugins_Should_Be_Active()
    {
        _pluginOptions.ShouldNotBeNull();
        var activeCount = _pluginOptions.Plugins.Count(p => p.IsActive);
        activeCount.ShouldBe(2);
    }

    [Fact]
    [Then("1 plugin should be marked as inactive", "UAC003")]
    public void One_Plugin_Should_Be_Inactive()
    {
        _pluginOptions.ShouldNotBeNull();
        var inactiveCount = _pluginOptions.Plugins.Count(p => !p.IsActive);
        inactiveCount.ShouldBe(1);
    }
}
