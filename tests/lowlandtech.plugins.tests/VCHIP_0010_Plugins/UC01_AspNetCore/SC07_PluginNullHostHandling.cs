namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC01_AspNetCore;

[Scenario(
    specId: "VCHIP-0010-UC01-SC07",
    title: "Plugin configuration with null host falls back gracefully",
    given: "Given a plugin is registered in a non-ASP.NET Core context",
    when: "When Configure is called with host set to null",
    then: "Then the plugin should detect the null host and skip WebApplication-specific configuration")]
public sealed class SC07_PluginNullHostHandling : WhenTestingForAsyncV2<AspNetCoreTestFixture>
{
    private TestPlugin? _plugin;
    private IServiceProvider? _serviceProvider;

    protected override AspNetCoreTestFixture For() => new AspNetCoreTestFixture();

    protected override Task GivenAsync(CancellationToken ct)
    {
        _plugin = new TestPlugin
        {
            Name = "Test Plugin",
            IsActive = true
        };

        var services = new ServiceCollection();
        services.AddPlugin(_plugin);

        _serviceProvider = services.BuildServiceProvider();

        return Task.CompletedTask;
    }

    protected override async Task WhenAsync(CancellationToken ct)
    {
        // Call Configure with null host (simulating non-ASP.NET Core context)
        await _plugin!.Configure(_serviceProvider!, host: null);
    }

    [Fact]
    [Then("The plugin should detect the null host", "UAC053")]
    public void Plugin_Should_Detect_Null_Host()
    {
        _plugin.ShouldNotBeNull();
        _plugin.ConfigureHost.ShouldBeNull();
    }

    [Fact]
    [Then("The plugin should skip WebApplication-specific configuration", "UAC054")]
    public void Plugin_Should_Skip_WebApp_Configuration()
    {
        _plugin.ShouldNotBeNull();
        _plugin.ConfigureCalled.ShouldBeTrue();
        // No routes should be registered since host was null
        _plugin.ConfigureHost.ShouldBeNull();
    }

    [Fact]
    [Then("The plugin should return Task.CompletedTask", "UAC055")]
    public async Task Plugin_Should_Return_Completed_Task()
    {
        // The Configure method should complete successfully
        var task = _plugin!.Configure(_serviceProvider!, host: null);
        task.ShouldNotBeNull();
        task.IsCompleted.ShouldBeTrue();
        await task; // Should not throw
    }
}
