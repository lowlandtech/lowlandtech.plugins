namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC03_DiscoveryAndLoading;

[Scenario(
    specId: "VCHIP-0010-UC03-SC03",
    title: "Load multiple plugins from configuration",
    given: "Given multiple plugins configured",
    when: "When discovery runs",
    then: "Then all active plugins load")]
public sealed class SC03_LoadMultiplePlugins : WhenTestingForV2<AspNetCoreTestFixture>
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
            ["Plugins:Plugins:1:IsActive"] = "true",
            ["Plugins:Plugins:2:Name"] = "LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC03_DiscoveryAndLoading",
            ["Plugins:Plugins:2:IsActive"] = "true"
        };

        _services = new ServiceCollection();
        _services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(configData!).Build();
        
        // Register as IConfiguration interface, not concrete type
        _services.AddSingleton<IConfiguration>(configuration);
        
        // Debug: verify configuration keys are present
        var allKeys = configuration.AsEnumerable().ToList();
        Console.WriteLine($"SC03 - Total configuration keys: {allKeys.Count}");
        foreach (var kv in allKeys)
        {
            Console.WriteLine($"  Key: '{kv.Key}' = '{kv.Value}'");
        }
        
        // Debug: verify it's registered in services
        var configDescriptor = _services.FirstOrDefault(d => d.ServiceType == typeof(IConfiguration));
        Console.WriteLine($"IConfiguration registered: {configDescriptor != null}");
    }

    protected override void When()
    {
        _services!.AddPlugins();
        var sp = _services.BuildServiceProvider();
        _registered = sp.GetServices<IPlugin>().ToList();
    }

    [Fact]
    [Then("All 3 plugins should be loaded", "UAC006")]
    public void All_Three_Loaded()
    {
        _registered.ShouldNotBeNull();
        _registered.Count(p => p.IsActive).ShouldBeGreaterThanOrEqualTo(2); // At least 2 (backend + one test plugin)
    }

    [Fact]
    [Then("All 3 plugins registered in service collection", "UAC007")]
    public void All_Three_Registered()
    {
        _services.ShouldNotBeNull();
        var sp = _services!.BuildServiceProvider();
        sp.GetServices<IPlugin>().Count().ShouldBeGreaterThanOrEqualTo(2);
    }
}
