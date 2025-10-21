namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC01_AspNetCore;

[Scenario(
    specId: "VCHIP-0010-UC01-SC07",
    title: "Multiple plugins register routes without conflicts",
    given: "Given multiple plugins are registered with different route patterns",
    when: "When app.UsePlugins() is called",
    then: "Then all routes should be registered successfully without conflicts")]
public sealed class SC07_MultiplePluginsRegisterRoutes : WhenTestingForV2<AspNetCoreTestFixture>
{
    private WebApplication? _app;
    private BackendTestPlugin? _backendPlugin;
    private FrontendTestPlugin? _frontendPlugin;
    private ApiTestPlugin? _apiPlugin;

    protected override AspNetCoreTestFixture For() => new AspNetCoreTestFixture();

    protected override void Given()
    {
        var builder = WebApplication.CreateBuilder();

        _backendPlugin = new BackendTestPlugin { Name = "Backend", IsActive = true };
        _frontendPlugin = new FrontendTestPlugin { Name = "Frontend", IsActive = true };
        _apiPlugin = new ApiTestPlugin { Name = "Api", IsActive = true };

        builder.Services.AddPlugin(_backendPlugin);
        builder.Services.AddPlugin(_frontendPlugin);
        builder.Services.AddPlugin(_apiPlugin);

        _app = builder.Build();
    }

    protected override void When()
    {
        _app!.UsePlugins();
    }

    [Fact]
    [Then("All routes should be registered successfully", "UAC019")]
    public void All_Routes_Should_Be_Registered()
    {
        _backendPlugin.ShouldNotBeNull();
        _frontendPlugin.ShouldNotBeNull();
        _apiPlugin.ShouldNotBeNull();

        _backendPlugin.ConfigureCalled.ShouldBeTrue();
        _frontendPlugin.ConfigureCalled.ShouldBeTrue();
        _apiPlugin.ConfigureCalled.ShouldBeTrue();
    }

    [Fact]
    [Then("Each plugin should receive the WebApplication host", "UAC020")]
    public void Each_Plugin_Should_Receive_WebApplication_Host()
    {
        _backendPlugin.ShouldNotBeNull();
        _frontendPlugin.ShouldNotBeNull();
        _apiPlugin.ShouldNotBeNull();

        _backendPlugin.ReceivedHost.ShouldBeOfType<WebApplication>();
        _frontendPlugin.ReceivedHost.ShouldBeOfType<WebApplication>();
        _apiPlugin.ReceivedHost.ShouldBeOfType<WebApplication>();
    }

    [Fact]
    [Then("All plugins should receive the same WebApplication instance", "UAC021")]
    public void All_Plugins_Should_Receive_Same_Host()
    {
        _backendPlugin.ShouldNotBeNull();
        _frontendPlugin.ShouldNotBeNull();
        _apiPlugin.ShouldNotBeNull();

        _backendPlugin.ReceivedHost.ShouldBe(_app);
        _frontendPlugin.ReceivedHost.ShouldBe(_app);
        _apiPlugin.ReceivedHost.ShouldBe(_app);
    }
}

public record RouteResponse(string Message, string PluginName);

[PluginId("770e8400-e29b-41d4-a716-446655440002")]
public class BackendTestPlugin : Plugin
{
    public bool ConfigureCalled { get; private set; }
    public object? ReceivedHost { get; private set; }

    public override void Install(IServiceCollection services) { }

    public override Task Configure(IServiceProvider provider, object? host = null)
    {
        ConfigureCalled = true;
        ReceivedHost = host;
        if (host is WebApplication app)
        {
            app.MapGet("/weatherforecast", () => new RouteResponse("Weather data", Name!));
        }
        return Task.CompletedTask;
    }
}

[PluginId("880e8400-e29b-41d4-a716-446655440003")]
public class FrontendTestPlugin : Plugin
{
    public bool ConfigureCalled { get; private set; }
    public object? ReceivedHost { get; private set; }

    public override void Install(IServiceCollection services) { }

    public override Task Configure(IServiceProvider provider, object? host = null)
    {
        ConfigureCalled = true;
        ReceivedHost = host;
        if (host is WebApplication app)
        {
            app.MapGet("/home", () => new RouteResponse("Home page", Name!));
        }
        return Task.CompletedTask;
    }
}

[PluginId("990e8400-e29b-41d4-a716-446655440004")]
public class ApiTestPlugin : Plugin
{
    public bool ConfigureCalled { get; private set; }
    public object? ReceivedHost { get; private set; }

    public override void Install(IServiceCollection services) { }

    public override Task Configure(IServiceProvider provider, object? host = null)
    {
        ConfigureCalled = true;
        ReceivedHost = host;
        if (host is WebApplication app)
        {
            app.MapGet("/api/data", () => new RouteResponse("API data", Name!));
        }
        return Task.CompletedTask;
    }
}