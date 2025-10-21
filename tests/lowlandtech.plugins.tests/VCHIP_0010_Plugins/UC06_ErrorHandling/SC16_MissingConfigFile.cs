namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC16",
    title: "Handle missing configuration file",
    given: "Given the appsettings.json file does not exist",
    when: "When the application attempts to load plugin configuration",
    then: "Then the configuration should return empty or default values")]
public sealed class SC16_MissingConfigFile : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private IServiceCollection? _services;
    private IConfiguration? _config;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        // simulate missing config by creating an empty configuration
        _config = new ConfigurationBuilder().Build();
        _services.AddSingleton<IConfiguration>(_config);
    }

    protected override void When()
    {
    }

    [Fact]
    [Then("The configuration should return empty or default values", "UAC050")]
    public void Config_Defaults()
    {
        var section = _config!.GetSection("Plugins");
        section.Exists().ShouldBeFalse();
    }

    [Fact]
    [Then("No plugins should be loaded from configuration", "UAC051")]
    public void No_Plugins()
    {
        _services!.AddPlugins();
        var sp = _services.BuildServiceProvider();
        sp.GetServices<IPlugin>().ShouldBeEmpty();
    }
}
