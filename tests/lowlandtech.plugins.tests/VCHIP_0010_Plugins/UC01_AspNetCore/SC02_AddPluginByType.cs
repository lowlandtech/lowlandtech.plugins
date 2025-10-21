namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC01_AspNetCore;

[Scenario(
    specId: "VCHIP-0010-UC01-SC02",
    title: "Add a specific plugin by type to service collection",
    given: "Given a WebApplicationBuilder is initialized and I have a BackendPlugin type",
    when: "When I call builder.Services.AddPlugin<TestPlugin>()",
    then: "Then an instance should be created, registered, and Install should be called")]
public sealed class SC02_AddPluginByType : WhenTestingForV2<AspNetCoreTestFixture>
{
    private IServiceCollection? _services;
    private TestPlugin? _plugin;
    private IServiceProvider? _serviceProvider;

    protected override AspNetCoreTestFixture For() => new AspNetCoreTestFixture();

    protected override void Given()
    {
        _services = new ServiceCollection();
    }

    protected override void When()
    {
        // Add plugin by type
        _services!.AddPlugin<TestPlugin>();

        // Build service provider
        _serviceProvider = _services.BuildServiceProvider();

        // Retrieve the plugin
        _plugin = _serviceProvider.GetServices<IPlugin>()
            .OfType<TestPlugin>()
            .FirstOrDefault();
    }

    [Fact]
    [Then("An instance of TestPlugin should be created", "UAC004")]
    public void Plugin_Instance_Should_Be_Created()
    {
        _plugin.ShouldNotBeNull();
        _plugin.ShouldBeOfType<TestPlugin>();
    }

    [Fact]
    [Then("The plugin should be registered in the service collection", "UAC005")]
    public void Plugin_Should_Be_Registered()
    {
        var plugins = _serviceProvider!.GetServices<IPlugin>().ToList();
        plugins.Count.ShouldBeGreaterThan(0);
        plugins.ShouldContain(p => p is TestPlugin);
    }

    [Fact]
    [Then("The plugin Install method should be called", "UAC006")]
    public void Plugin_Install_Should_Be_Called()
    {
        _plugin.ShouldNotBeNull();
        _plugin.InstallCalled.ShouldBeTrue();
    }
}
