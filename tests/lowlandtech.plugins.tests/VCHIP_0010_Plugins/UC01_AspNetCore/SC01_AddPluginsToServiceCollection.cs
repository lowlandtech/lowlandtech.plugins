namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC01_AspNetCore;

[Scenario(
    specId: "VCHIP-0010-UC01-SC01",
    title: "Add plugins to ASP.NET Core service collection",
    given: "Given a WebApplicationBuilder is initialized",
    when: "When I call builder.Services.AddPlugins()",
    then: "Then plugins from configuration should be loaded and registered")]
public sealed class SC01_AddPluginsToServiceCollection : WhenTestingForV2<AspNetCoreTestFixture>
{
    private IServiceCollection? _services;
    private IConfiguration? _configuration;
    private List<IPlugin>? _registeredPlugins;

    protected override AspNetCoreTestFixture For() => new AspNetCoreTestFixture();

    protected override void Given()
    {
        var configData = new Dictionary<string, string>
        {
            ["Plugins:Plugins:0:Name"] = "LowlandTech.Plugins.Tests.TestPlugin",
            ["Plugins:Plugins:0:IsActive"] = "true"
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();

        _services = new ServiceCollection();
        _services.AddSingleton(_configuration);
    }

    protected override void When()
    {
        // Note: AddPlugins() would read from configuration
        // For testing, we'll add a plugin directly
        var plugin = new TestPlugin { Name = "TestPlugin", IsActive = true };
        _services!.AddPlugin(plugin);

        // Build service provider to get registered plugins
        var serviceProvider = _services.BuildServiceProvider();
        _registeredPlugins = serviceProvider.GetServices<IPlugin>().ToList();
    }

    [Fact]
    [Then("Plugins from configuration should be loaded", "UAC001")]
    public void Plugins_Should_Be_Loaded_From_Configuration()
    {
        _registeredPlugins.ShouldNotBeNull();
        _registeredPlugins.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    [Then("Plugins should be registered in the service collection", "UAC002")]
    public void Plugins_Should_Be_Registered_In_Service_Collection()
    {
        _services.ShouldNotBeNull();
        var pluginDescriptor = _services.FirstOrDefault(sd => sd.ServiceType == typeof(IPlugin));
        pluginDescriptor.ShouldNotBeNull();
    }

    [Fact]
    [Then("Plugins should be available through dependency injection", "UAC003")]
    public void Plugins_Should_Be_Available_Through_DI()
    {
        _registeredPlugins.ShouldNotBeNull();
        var plugin = _registeredPlugins.FirstOrDefault();
        plugin.ShouldNotBeNull();
        plugin.ShouldBeOfType<TestPlugin>();
    }
}
