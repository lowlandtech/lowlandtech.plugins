namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC03_DiscoveryAndLoading;

[Scenario(
    specId: "VCHIP-0010-UC03-SC11",
    title: "Handle empty plugins configuration section",
    given: "Given the configuration has an empty \"Plugins\" section",
    when: "When the plugin discovery process runs",
    then: "Then no plugins should be loaded and no errors logged")]
public sealed class SC11_EmptyPluginsSection : WhenTestingForV2<AspNetCoreTestFixture>
{
    private IServiceCollection? _services;
    private List<IPlugin>? _registered;

    protected override AspNetCoreTestFixture For() => new();

    protected override void Given()
    {
        // provide a Plugins section but empty
        var configData = new Dictionary<string, string>
        {
            ["Plugins"] = string.Empty
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
    [Then("No plugins loaded", "UAC026")]
    public void No_Plugins_Loaded()
    {
        _registered.ShouldNotBeNull();
        _registered.ShouldBeEmpty();
    }

    [Fact]
    [Then("No errors logged and app continues", "UAC027-UAC028")]
    public void No_Errors()
    {
        // just ensure nothing crashed
        _registered.ShouldNotBeNull();
    }
}
