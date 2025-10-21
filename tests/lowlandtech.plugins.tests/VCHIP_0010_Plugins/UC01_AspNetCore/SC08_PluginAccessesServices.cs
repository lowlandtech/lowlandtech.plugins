namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC01_AspNetCore;

[Scenario(
    specId: "VCHIP-0010-UC01-SC08",
    title: "Plugin accesses ASP.NET Core services during Configure",
    given: "Given a plugin is registered and the application has ILogger and IConfiguration",
    when: "When the plugin Configure method executes",
    then: "Then the plugin should be able to resolve and use ASP.NET Core services")]
public sealed class SC08_PluginAccessesServices : WhenTestingForV2<AspNetCoreTestFixture>
{
    private IServiceProvider? _serviceProvider;
    private ILogger<TestPlugin>? _logger;
    private IConfiguration? _configuration;

    protected override AspNetCoreTestFixture For() => new AspNetCoreTestFixture();

    protected override void Given()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ASPNETCORE_ENVIRONMENT", "Development" }
        });

        var plugin = new TestPlugin
        {
            Name = "Test Plugin",
            IsActive = true
        };

        builder.Services.AddPlugin(plugin);

        var app = builder.Build();
        app.UsePlugins();

        _serviceProvider = app.Services;
    }

    protected override void When()
    {
        // Resolve services from the service provider
        _logger = _serviceProvider!.GetService<ILogger<TestPlugin>>();
        _configuration = _serviceProvider.GetService<IConfiguration>();
    }

    [Fact]
    [Then("The plugin should be able to resolve ILogger from the service provider", "UAC022")]
    public void Plugin_Should_Resolve_ILogger()
    {
        _logger.ShouldNotBeNull();
    }

    [Fact]
    [Then("The plugin should be able to resolve IConfiguration", "UAC023")]
    public void Plugin_Should_Resolve_IConfiguration()
    {
        _configuration.ShouldNotBeNull();
    }

    [Fact]
    [Then("The plugin should be able to use these services for configuration", "UAC024")]
    public void Plugin_Should_Use_Services()
    {
        // Verify services are functional
        _logger.ShouldNotBeNull();
        _configuration.ShouldNotBeNull();

        // Test that configuration can be accessed
        var envName = _configuration["ASPNETCORE_ENVIRONMENT"];
        envName.ShouldNotBeNullOrEmpty();
    }
}
