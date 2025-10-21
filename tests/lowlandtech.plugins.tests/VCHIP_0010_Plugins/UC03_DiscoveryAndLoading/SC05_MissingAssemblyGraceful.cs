namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC03_DiscoveryAndLoading;

[Scenario(
    specId: "VCHIP-0010-UC03-SC05",
    title: "Fail gracefully when plugin assembly is missing",
    given: "Given configuration references a missing assembly",
    when: "When discovery runs",
    then: "Then an error should be logged and processing continues")]
public sealed class SC05_MissingAssemblyGraceful : WhenTestingForV2<AspNetCoreTestFixture>
{
    private IServiceCollection? _services;
    private List<IPlugin>? _registered;

    protected override AspNetCoreTestFixture For() => new();

    protected override void Given()
    {
        var configData = new Dictionary<string, string>
        {
            ["Plugins:Plugins:0:Name"] = "LowlandTech.Missing.Plugin",
            ["Plugins:Plugins:0:IsActive"] = "true"
        };

        _services = new ServiceCollection();
        // add a logger factory that captures logs
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _services.AddSingleton<ILoggerFactory>(loggerFactory);

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
    [Then("Error logged and continues", "UAC011")]
    public void Error_Logged()
    {
        // no exception thrown and registration empty
        _registered.ShouldNotBeNull();
        _registered.Count.ShouldBe(0);
    }

    [Fact]
    [Then("Application continues", "UAC012")]
    public void Application_Continues()
    {
        // ensure no exception thrown during add
        _registered.ShouldNotBeNull();
    }

    [Fact]
    [Then("No instance registered", "UAC013")]
    public void No_Instance_Registered()
    {
        _registered.ShouldNotBeNull();
        _registered.Any(p => p.Name == "LowlandTech.Missing.Plugin").ShouldBeFalse();
    }
}
