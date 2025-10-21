namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC00_Configuration;

[Scenario(
    specId: "VCHIP-0010-UC00-SC08",
    title: "Configuration binding with IConfiguration",
    given: "Given I have an IConfiguration instance with plugin settings",
    when: "When I bind the configuration to PluginOptions using the 'Plugins' section",
    then: "Then the PluginOptions should be populated correctly with all plugin configurations deserialized")]
public sealed class SC08_ConfigurationBinding : WhenTestingForV2<ConfigurationTestFixture>
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
            ["Plugins:Plugins:1:IsActive"] = "false"
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();
    }

    protected override void When()
    {
        _pluginOptions = _configuration
            .GetSection("Plugins")
            .Get<PluginOptions>();
    }

    [Fact]
    [Then("The PluginOptions should be populated correctly", "UAC001")]
    public void PluginOptions_Should_Be_Populated()
    {
        _pluginOptions.ShouldNotBeNull();
        _pluginOptions.Plugins.ShouldNotBeNull();
        _pluginOptions.Plugins.Count.ShouldBe(2);
    }

    [Fact]
    [Then("All plugin configurations should be deserialized properly", "UAC002")]
    public void All_Plugin_Configs_Should_Be_Deserialized()
    {
        _pluginOptions.ShouldNotBeNull();

        var backend = _pluginOptions.Plugins.FirstOrDefault(p => p.Name == "LowlandTech.Sample.Backend");
        backend.ShouldNotBeNull();
        backend.IsActive.ShouldBeTrue();

        var frontend = _pluginOptions.Plugins.FirstOrDefault(p => p.Name == "LowlandTech.Sample.Frontend");
        frontend.ShouldNotBeNull();
        frontend.IsActive.ShouldBeFalse();
    }
}
