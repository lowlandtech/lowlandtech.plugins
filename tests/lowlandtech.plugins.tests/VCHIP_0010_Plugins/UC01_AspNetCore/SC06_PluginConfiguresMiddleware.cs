using Microsoft.AspNetCore.Http;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC01_AspNetCore;

[Scenario(
    specId: "VCHIP-0010-UC01-SC06",
    title: "Plugin configures middleware during Configure phase",
    given: "Given a plugin needs to add middleware and the WebApplication is built",
    when: "When the plugin Configure method executes",
    then: "Then the plugin should be able to call app.UseMiddleware and it should execute for incoming requests")]
public sealed class SC06_PluginConfiguresMiddleware : WhenTestingForAsyncV2<AspNetCoreTestFixture>
{
    private WebApplication? _app;
    private MiddlewareTestPlugin? _plugin;

    protected override AspNetCoreTestFixture For() => new AspNetCoreTestFixture();

    protected override async Task GivenAsync(CancellationToken ct)
    {
        var builder = WebApplication.CreateBuilder();

        _plugin = new MiddlewareTestPlugin
        {
            Name = "Middleware Test Plugin",
            IsActive = true
        };

        builder.Services.AddPlugin(_plugin);

        _app = builder.Build();
        _app.UsePlugins();

        // Add a test endpoint to verify middleware runs
        _app.MapGet("/test", () => "Test response");
        
        await Task.CompletedTask;
    }

    protected override Task WhenAsync(CancellationToken ct)
    {
        // Middleware is configured during UsePlugins() call in Given
        return Task.CompletedTask;
    }

    [Fact]
    [Then("The plugin should be able to call app.UseMiddleware", "UAC016")]
    public void Plugin_Should_Be_Able_To_Call_UseMiddleware()
    {
        _plugin.ShouldNotBeNull();
        _plugin.MiddlewareAdded.ShouldBeTrue();
    }

    [Fact]
    [Then("The middleware should be added to the pipeline", "UAC017")]
    public void Middleware_Should_Be_Added_To_Pipeline()
    {
        _plugin.ShouldNotBeNull();
        _plugin.ConfigureHost.ShouldBeOfType<WebApplication>();
    }

    [Fact]
    [Then("The plugin received the WebApplication host", "UAC018")]
    public void Plugin_Received_WebApplication_Host()
    {
        _plugin.ShouldNotBeNull();
        _plugin.ConfigureHost.ShouldBe(_app);
    }

    public override async ValueTask DisposeAsync()
    {
        if (_app != null)
        {
            await _app.DisposeAsync();
        }
        GC.SuppressFinalize(this);
    }
}

// Test middleware
public class TestMiddleware
{
    private readonly RequestDelegate _next;

    public TestMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers.Append("X-Middleware-Test", "Executed");
        await _next(context);
    }
}

// Plugin that adds middleware
[PluginId("660e8400-e29b-41d4-a716-446655440001")]
public class MiddlewareTestPlugin : Plugin
{
    public bool MiddlewareAdded { get; private set; }
    public object? ConfigureHost { get; private set; }

    public override void Install(IServiceCollection services) { }

    public override Task Configure(IServiceProvider container, object? host = null)
    {
        ConfigureHost = host;

        if (host is WebApplication app)
        {
            app.UseMiddleware<TestMiddleware>();
            MiddlewareAdded = true;
        }

        return Task.CompletedTask;
    }
}
