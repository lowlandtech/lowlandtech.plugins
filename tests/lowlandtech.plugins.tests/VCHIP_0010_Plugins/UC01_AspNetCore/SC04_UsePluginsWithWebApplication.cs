namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC01_AspNetCore;

[Scenario(
    specId: "VCHIP-0010-UC01-SC04",
    title: "Use plugins with WebApplication host",
    given: "Given plugins have been registered and the WebApplication has been built",
    when: "When I call app.UsePlugins()",
    then: "Then Configure should be called on each plugin with service provider and host")]
public sealed class SC04_UsePluginsWithWebApplication : WhenTestingForV2<AspNetCoreTestFixture>
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
    }

    protected override void When()
    {
        _app!.UsePlugins();
    }

    [Fact]
    [Then("The Configure method should be called on each plugin", "UAC010")]
    public void Configure_Should_Be_Called()
    {
        _plugin.ShouldNotBeNull();
        _plugin.ConfigureCalled.ShouldBeTrue();
    }

    [Fact]
    [Then("Each plugin should receive the service provider", "UAC011")]
    public void Plugin_Should_Receive_Service_Provider()
    {
        _plugin.ShouldNotBeNull();
        _plugin.ConfigureServiceProvider.ShouldNotBeNull();
    }

    [Fact]
    [Then("Each plugin should receive the WebApplication as the host parameter", "UAC012")]
    public void Plugin_Should_Receive_WebApplication_Host()
    {
        _plugin.ShouldNotBeNull();
        _plugin.ConfigureHost.ShouldNotBeNull();
        _plugin.ConfigureHost.ShouldBeOfType<WebApplication>();
    }
}
