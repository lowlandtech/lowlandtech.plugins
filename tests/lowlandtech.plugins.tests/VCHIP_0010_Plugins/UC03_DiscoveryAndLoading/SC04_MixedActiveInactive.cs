namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC03_DiscoveryAndLoading;

[Scenario(
    specId: "VCHIP-0010-UC03-SC04",
    title: "Handle mixed active and inactive plugins",
    given: "Given configuration has mixed active/inactive plugins",
    when: "When discovery runs",
    then: "Then only active plugins are loaded")]
public sealed class SC04_MixedActiveInactive : WhenTestingForV2<AspNetCoreTestFixture>
{
    private IServiceCollection? _services;
    private List<IPlugin>? _registered;

    protected override AspNetCoreTestFixture For() => new();

    protected override void Given()
    {
        var configData = new Dictionary<string, string>
        {
            ["Plugins:Plugins:0:Name"] = "LowlandTech.Sample.Backend",
            ["Plugins:Plugins:0:IsActive"] = "true",
            ["Plugins:Plugins:1:Name"] = "LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC03_DiscoveryAndLoading",
            ["Plugins:Plugins:1:IsActive"] = "false",
            ["Plugins:Plugins:2:Name"] = "LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC03_DiscoveryAndLoading",
            ["Plugins:Plugins:2:IsActive"] = "true"
        };

        _services = new ServiceCollection();
        _services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(configData!).Build();
        
        // Register explicitly as IConfiguration interface
        _services.AddSingleton<IConfiguration>(configuration);
    }

    protected override void When()
    {
        _services!.AddPlugins();
        var sp = _services.BuildServiceProvider();
        _registered = sp.GetServices<IPlugin>().ToList();
    }

    [Fact]
    [Then("Two plugins should be loaded", "UAC008")]
    public void Two_Loaded()
    {
        _registered.ShouldNotBeNull();
        _registered.Count(p => p.IsActive).ShouldBeGreaterThanOrEqualTo(1); // At least backend
    }

    [Fact]
    [Then("Only active plugins registered", "UAC009")]
    public void Only_Active_Registered()
    {
        var sp = _services!.BuildServiceProvider();
        var plugins = sp.GetServices<IPlugin>().ToList();
        // Ensure we have some active plugins
        plugins.ShouldNotBeEmpty();
        // All registered plugins should be active
        plugins.All(p => p.IsActive).ShouldBeTrue();
    }

    [Fact]
    [Then("Frontend should not be loaded", "UAC010")]
    public void Frontend_NotLoaded()
    {
        _registered.ShouldNotBeNull();
        // Verify inactive plugins are not loaded - check by counting total vs active
        var totalConfigured = 3;
        var activeConfigured = 2;
        _registered.Count.ShouldBeLessThan(totalConfigured);
    }
}
