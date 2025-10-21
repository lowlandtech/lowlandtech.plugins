namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC03_DiscoveryAndLoading;

[Scenario(
    specId: "VCHIP-0010-UC03-SC01",
    title: "Load a single plugin from configuration",
    given: "Given the configuration contains a plugin entry for \"LowlandTech.Sample.Backend\"",
    when: "When the plugin discovery process runs",
    then: "Then the plugin should be loaded and registered")]
public sealed class SC01_LoadSinglePlugin : WhenTestingForV2<AspNetCoreTestFixture>
{
    private IServiceCollection? _services;
    private List<IPlugin>? _registered;
    private IConfiguration? _configuration;

    protected override AspNetCoreTestFixture For() => new();

    protected override void Given()
    {
        var configData = new Dictionary<string, string>
        {
            ["Plugins:Plugins:0:Name"] = "LowlandTech.Sample.Backend",
            ["Plugins:Plugins:0:IsActive"] = "true"
        };

        _services = new ServiceCollection();
        
        // Add logger factory for plugin discovery debugging
        _services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        
        _configuration = new ConfigurationBuilder().AddInMemoryCollection(configData!).Build();
        
        // Register explicitly as IConfiguration interface
        _services.AddSingleton<IConfiguration>(_configuration);
        
        // Debug: verify configuration keys are present
        var allKeys = _configuration.AsEnumerable().ToList();
        Console.WriteLine($"Total configuration keys: {allKeys.Count}");
        foreach (var kv in allKeys)
        {
            Console.WriteLine($"  Key: '{kv.Key}' = '{kv.Value}'");
        }
    }

    protected override void When()
    {
        // call AddPlugins which performs discovery
        // Use AspNetCore variant
        _services!.AddPlugins();
        var sp = _services.BuildServiceProvider();
        _registered = sp.GetServices<IPlugin>().ToList();
    }

    [Fact]
    [Then("Plugin loaded and registered", "UAC001")]
    public void Plugin_Should_Be_Loaded()
    {
        _registered.ShouldNotBeNull();
        _registered.Count.ShouldBeGreaterThan(0);
        _registered.Any(p => p.Name == "LowlandTech.Sample.Backend").ShouldBeTrue();
    }

    [Fact]
    [Then("Registered in service collection", "UAC002")]
    public void Registered_In_ServiceCollection()
    {
        var desc = _services!.FirstOrDefault(d => d.ServiceType == typeof(IPlugin));
        desc.ShouldNotBeNull();
    }

    [Fact]
    [Then("Plugin IsActive true", "UAC003")]
    public void Plugin_IsActive()
    {
        var plugin = _registered!.First(p => p.Name == "LowlandTech.Sample.Backend");
        plugin.IsActive.ShouldBeTrue();
    }
}
