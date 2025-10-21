namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC01_AspNetCore;

[Scenario(
    specId: "VCHIP-0010-UC01-SC03",
    title: "Add a plugin instance to service collection",
    given: "Given a WebApplicationBuilder is initialized and I have a TestPlugin instance",
    when: "When I call builder.Services.AddPlugin(pluginInstance)",
    then: "Then the plugin instance should be registered directly and Install should be called")]
public sealed class SC03_AddPluginInstance : WhenTestingForV2<AspNetCoreTestFixture>
{
    private IServiceCollection? _services;
    private TestPlugin? _originalPlugin;
    private IServiceProvider? _serviceProvider;
    private TestPlugin? _retrievedPlugin;

    protected override AspNetCoreTestFixture For() => new AspNetCoreTestFixture();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _originalPlugin = new TestPlugin
        {
            Name = "My Test Plugin",
            IsActive = true
        };
    }

    protected override void When()
    {
        // Add plugin instance
        _services!.AddPlugin(_originalPlugin!);

        // Build service provider
        _serviceProvider = _services.BuildServiceProvider();

        // Retrieve the plugin
        _retrievedPlugin = _serviceProvider.GetServices<IPlugin>()
            .OfType<TestPlugin>()
            .FirstOrDefault();
    }

    [Fact]
    [Then("The plugin instance should be registered directly", "UAC007")]
    public void Plugin_Instance_Should_Be_Registered()
    {
        _retrievedPlugin.ShouldNotBeNull();
        _retrievedPlugin.ShouldBe(_originalPlugin); // Same instance
    }

    [Fact]
    [Then("The plugin Install method should be called", "UAC008")]
    public void Plugin_Install_Should_Be_Called()
    {
        _originalPlugin.ShouldNotBeNull();
        _originalPlugin.InstallCalled.ShouldBeTrue();
    }

    [Fact]
    [Then("The plugin should be available for dependency injection", "UAC009")]
    public void Plugin_Should_Be_Available_For_DI()
    {
        var service = _serviceProvider!.GetService<TestService>();
        service.ShouldNotBeNull();
        service.GetMessage().ShouldBe("Test service from plugin");
    }
}
