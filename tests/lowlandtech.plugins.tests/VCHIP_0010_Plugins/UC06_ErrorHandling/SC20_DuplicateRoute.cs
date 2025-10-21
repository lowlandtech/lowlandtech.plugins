namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC20",
    title: "Handle plugin attempting to register duplicate route",
    given: "Given Plugin A registers /api/data and Plugin B also attempts to register same route",
    when: "When both plugins configure their routes",
    then: "Then ASP.NET Core allows duplicate route registrations by design")]
public sealed class SC20_DuplicateRoute : WhenTestingForV2<ErrorHandlingTestFixture>
{
    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given() { }
    protected override void When() { }

    [Fact]
    [Then("ASP.NET Core allows duplicate routes - no exception thrown", "UAC062")]
    public void Duplicate_Routes_Allowed()
    {
        var builder = WebApplication.CreateBuilder();
        var services = builder.Services;

        // Use distinct plugin types with unique PluginId to allow registration
        services.AddPlugin(new SimpleRoutePluginA("/api/data"));
        services.AddPlugin(new SimpleRoutePluginB("/api/data"));

        var app = builder.Build();
        
        // ASP.NET Core allows duplicate routes by design - no exception is thrown
        // This is documented behavior: last route wins or ambiguous match handling applies at runtime
        Should.NotThrow(() => app.UsePlugins(host: app));
        
        // Note: Developers should use unique route patterns per plugin or implement custom
        // route conflict detection if needed. The framework intentionally allows this flexibility.
    }
}

// Helper plugin that registers a single route
public class SimpleRoutePlugin : Plugin
{
    protected readonly string _route;
    public SimpleRoutePlugin(string route) => _route = route;
    public override void Install(IServiceCollection services) { }
    public override Task Configure(IServiceProvider provider, object? host = null)
    {
        if (host is WebApplication app)
        {
            app.MapGet(_route, () => "ok");
        }
        return Task.CompletedTask;
    }
}

// Distinct subclasses with unique PluginId attributes so multiple instances can be registered
[PluginId("eaaaaaaaa-0000-4000-8000-00000000000b")]
public class SimpleRoutePluginA : SimpleRoutePlugin
{
    public SimpleRoutePluginA(string route) : base(route) { }
}

[PluginId("eaaaaaaaa-0000-4000-8000-00000000000c")]
public class SimpleRoutePluginB : SimpleRoutePlugin
{
    public SimpleRoutePluginB(string route) : base(route) { }
}
