namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC03_DiscoveryAndLoading;

[Scenario(
    specId: "VCHIP-0010-UC03-SC02",
    title: "Skip loading inactive plugins",
    given: "Given the configuration contains an inactive plugin",
    when: "When discovery runs",
    then: "Then the plugin should not be loaded")]
public sealed class SC02_SkipInactivePlugin : WhenTestingForV2<AspNetCoreTestFixture>
{
    private IServiceCollection? _services;
    private List<IPlugin>? _registered;

    protected override AspNetCoreTestFixture For() => new();

    protected override void Given()
    {
        var configData = new Dictionary<string, string>
        {
            ["Plugins:Plugins:0:Name"] = "LowlandTech.Sample.Backend",
            ["Plugins:Plugins:0:IsActive"] = "false"
        };

        _services = new ServiceCollection();
        _services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        
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
    [Then("Plugin should not be loaded", "UAC004")]
    public void Plugin_Not_Loaded()
    {
        _registered.ShouldNotBeNull();
        _registered.Any(p => p.Name == "LowlandTech.Sample.Backend").ShouldBeFalse();
    }

    [Fact]
    [Then("Not registered in service collection", "UAC005")]
    public void Not_Registered()
    {
        var desc = _services!.FirstOrDefault(d => d.ServiceType == typeof(IPlugin));
        // ensure either no IPlugin descriptor or none matches backend
        if (desc is null) return;
        var sp = _services.BuildServiceProvider();
        var found = sp.GetServices<IPlugin>().Any(p => p.Name == "LowlandTech.Sample.Backend");
        found.ShouldBeFalse();
    }
}
