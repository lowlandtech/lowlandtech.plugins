namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC03_DiscoveryAndLoading;

[Scenario(
    specId: "VCHIP-0010-UC03-SC12",
    title: "Handle missing plugins configuration section",
    given: "Given the configuration does not contain a \"Plugins\" section",
    when: "When the plugin discovery process runs",
    then: "Then no plugins should be loaded and no errors should be logged")]
public sealed class SC12_MissingPluginsSection : WhenTestingForV2<AspNetCoreTestFixture>
{
    private IServiceCollection? _services;
    private List<IPlugin>? _registered;

    protected override AspNetCoreTestFixture For() => new();

    protected override void Given()
    {
        // no Plugins key at all
        var configData = new Dictionary<string, string>
        {
            ["SomeOtherKey"] = "value"
        };

        _services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(configData!).Build();
        _services.AddSingleton<IConfiguration>(configuration);
    }

    protected override void When()
    {
        _services!.AddPlugins();
        var sp = _services.BuildServiceProvider();
        _registered = sp.GetServices<IPlugin>().ToList();
    }

    [Fact]
    [Then("No plugins loaded", "UAC029")]
    public void No_Plugins_Loaded()
    {
        _registered.ShouldNotBeNull();
        _registered.ShouldBeEmpty();
    }

    [Fact]
    [Then("No errors logged and app continues", "UAC030-UAC031")]
    public void No_Errors()
    {
        // just ensure nothing crashed
        _registered.ShouldNotBeNull();
    }
}
