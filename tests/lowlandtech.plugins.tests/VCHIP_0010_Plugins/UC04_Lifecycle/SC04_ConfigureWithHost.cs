using LowlandTech.Plugins.Tests.Fixtures;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

[Scenario(
    specId: "VCHIP-0010-UC04-SC04",
    title: "Execute Configure phase with host object (ASP.NET Core)",
    given: "Given a plugin has been installed in an ASP.NET Core application and the WebApplication has been built",
    when: "When the Configure method is called with the service provider and WebApplication host",
    then: "Then the plugin should have access to the WebApplication instance")]
public sealed class SC04_ConfigureWithHost : WhenTestingForV2<AspNetCoreTestFixture>
{
    private IServiceCollection? _services;
    private TestLifecyclePlugin? _plugin;
    private WebApplication? _app;
    private Task? _configureTask;

    protected override AspNetCoreTestFixture For() => new();

    protected override void Given()
    {
        var builder = WebApplication.CreateBuilder();
        _services = builder.Services;
        
        _plugin = new TestLifecyclePlugin();
        _services.AddPlugin(_plugin);
        
        _app = builder.Build();
    }

    protected override void When()
    {
        _configureTask = _plugin!.Configure(_app!.Services, host: _app);
    }

    [Fact]
    [Then("The plugin should have access to the WebApplication instance", "UAC010")]
    public async Task Plugin_Should_Have_Access_To_WebApplication()
    {
        await _configureTask!;
        _plugin!.HostReceived.ShouldNotBeNull();
        _plugin.HostReceived.ShouldBeOfType<WebApplication>();
    }

    [Fact]
    [Then("The plugin should be able to configure routes", "UAC011")]
    public async Task Plugin_Should_Configure_Routes()
    {
        await _configureTask!;
        
        // Verify the test plugin registered a route
        _plugin!.RouteConfigured.ShouldBeTrue();
    }

    [Fact]
    [Then("The plugin should be able to configure middleware", "UAC012")]
    public async Task Plugin_Should_Configure_Middleware()
    {
        await _configureTask!;
        
        // Verify the test plugin configured middleware
        _plugin!.MiddlewareConfigured.ShouldBeTrue();
    }

    [Fact]
    [Then("The Configure method should complete successfully", "UAC013")]
    public async Task Configure_Should_Complete_Successfully()
    {
        await _configureTask!;
        _plugin!.ConfigureCalled.ShouldBeTrue();
        _plugin.ConfigureException.ShouldBeNull();
    }
}
