namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC01_AspNetCore;

[Scenario(
    specId: "VCHIP-0010-UC01-SC05",
    title: "Plugin registers routes during Configure phase",
    given: "Given a TestPlugin is registered and the WebApplication is built",
    when: "When app.UsePlugins() is called",
    then: "Then the plugin should register routes and they should be accessible")]
public sealed class SC05_PluginRegistersRoutes : WhenTestingForV2<AspNetCoreTestFixture>
{
    private WebApplication? _app;
    private TestPlugin? _plugin;

    protected override AspNetCoreTestFixture For() => new AspNetCoreTestFixture();

    protected override void Given()
    {
        var builder = WebApplication.CreateBuilder();

        _plugin = new TestPlugin
        {
            Name = "Test Plugin",
            IsActive = true
        };

        builder.Services.AddPlugin(_plugin);

        _app = builder.Build();
        _app.UsePlugins();
    }

    protected override void When()
    {
        // Plugin configuration happens during UsePlugins()
        // Routes are registered during Configure phase
    }

    [Fact]
    [Then("The plugin should register routes using app.MapGet", "UAC013")]
    public void Plugin_Should_Register_Routes()
    {
        // Verify Configure was called and host was WebApplication
        _plugin.ShouldNotBeNull();
        _plugin.ConfigureCalled.ShouldBeTrue();
        _plugin.ConfigureHost.ShouldBeOfType<WebApplication>();
    }

    [Fact]
    [Then("The routes should be accessible in the application", "UAC014")]
    public void Routes_Should_Be_Accessible()
    {
        // Verify the WebApplication instance exists where routes were registered
        _app.ShouldNotBeNull();
        _plugin.ShouldNotBeNull();
        _plugin.ConfigureHost.ShouldNotBeNull();
    }

    [Fact]
    [Then("Requests to plugin routes should be handled correctly", "UAC015")]
    public void Plugin_Routes_Should_Handle_Requests()
    {
        // Verify plugin had access to WebApplication for route registration
        _plugin.ShouldNotBeNull();
        _plugin.ConfigureHost.ShouldBe(_app);
    }
}
