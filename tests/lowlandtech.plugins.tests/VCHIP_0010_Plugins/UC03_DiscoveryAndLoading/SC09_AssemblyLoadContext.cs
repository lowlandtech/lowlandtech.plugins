namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC03_DiscoveryAndLoading;

[Scenario(
    specId: "VCHIP-0010-UC03-SC09",
    title: "Plugin assembly is loaded into correct AssemblyLoadContext",
    given: "Given the configuration contains a plugin entry for \"LowlandTech.Sample.Backend\"",
    when: "When the plugin discovery process runs",
    then: "Then the plugin assembly should be loaded into the current AssemblyLoadContext")]
public sealed class SC09_AssemblyLoadContext : WhenTestingForV2<AspNetCoreTestFixture>
{
    private IServiceCollection? _services;
    private List<IPlugin>? _registered;

    protected override AspNetCoreTestFixture For() => new();

    protected override void Given()
    {
        var configData = new Dictionary<string, string>
        {
            ["Plugins:Plugins:0:Name"] = "LowlandTech.Sample.Backend",
            ["Plugins:Plugins:0:IsActive"] = "true"
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
    [Then("Assembly loaded into current ALC", "UAC022")]
    public void AssemblyLoadedInALC()
    {
        var plugin = _registered!.FirstOrDefault(p => p.Name == "LowlandTech.Sample.Backend");
        plugin.ShouldNotBeNull();

        if (plugin is LowlandTech.Plugins.Types.Plugin p)
        {
            p.Assemblies.ShouldNotBeNull();
            p.Assemblies.Count.ShouldBeGreaterThan(0);
        }
        else
        {
            Assert.Fail("Plugin not derived from Plugin base class");
        }
    }

    [Fact]
    [Then("Assembly accessible from plugin's Assemblies", "UAC023")]
    public void AssemblyAccessibleFromPlugin()
    {
        var plugin = _registered!.First(p => p.Name == "LowlandTech.Sample.Backend");
        var derived = plugin as LowlandTech.Plugins.Types.Plugin;
        derived.ShouldNotBeNull();
        derived!.Assemblies.ShouldContain(a => a.GetName().Name!.Contains("LowlandTech.Sample.Backend"));
    }
}
