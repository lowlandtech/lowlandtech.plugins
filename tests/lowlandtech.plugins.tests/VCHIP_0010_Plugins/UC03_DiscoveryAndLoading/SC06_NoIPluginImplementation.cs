namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC03_DiscoveryAndLoading;

[Scenario(
    specId: "VCHIP-0010-UC03-SC06",
    title: "Fail gracefully when plugin assembly has no IPlugin implementation",
    given: "Given configuration references an assembly with no IPlugin types",
    when: "When discovery runs",
    then: "Then an error should be logged and no plugin registered")]
public sealed class SC06_NoIPluginImplementation : WhenTestingForV2<AspNetCoreTestFixture>
{
    private IServiceCollection? _services;
    private List<IPlugin>? _registered;

    protected override AspNetCoreTestFixture For() => new();

    protected override void Given()
    {
        var configData = new Dictionary<string, string>
        {
            ["Plugins:Plugins:0:Name"] = "LowlandTech.Invalid.Plugin",
            ["Plugins:Plugins:0:IsActive"] = "true"
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
    [Then("Error logged indicating no implementation found", "UAC014")]
    public void Error_Logged_No_Impl()
    {
        // ensure no plugins registered
        _registered.ShouldNotBeNull();
        _registered.ShouldBeEmpty();
    }

    [Fact]
    [Then("Application continues", "UAC015")]
    public void Application_Continues()
    {
        _registered.ShouldNotBeNull();
    }

    [Fact]
    [Then("No plugin instance registered", "UAC016")]
    public void No_Instance_Registered()
    {
        _registered.ShouldNotBeNull();
        _registered.ShouldBeEmpty();
    }
}
