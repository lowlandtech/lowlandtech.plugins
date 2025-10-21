using LowlandTech.Plugins.Tests.Fixtures;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

[Scenario(
    specId: "VCHIP-0010-UC04-SC16",
    title: "Plugin Assemblies collection is available during lifecycle",
    given: "Given a plugin has loaded assemblies in its Assemblies collection",
    when: "When the plugin executes through its lifecycle phases",
    then: "Then the Assemblies collection should be accessible in Install")]
public sealed class SC16_AssembliesCollectionAvailable : WhenTestingForV2<LifecycleTestFixture>
{
    private IServiceCollection? _services;
    private AssembliesAwarePlugin? _plugin;
    private IServiceProvider? _provider;

    protected override LifecycleTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _plugin = new AssembliesAwarePlugin();

        // simulate loaded assemblies
        _plugin.Assemblies.Add(typeof(AssembliesAwarePlugin).Assembly);

        _services.AddPlugin(_plugin);
        _provider = _services.BuildServiceProvider();
    }

    protected override void When()
    {
        _plugin!.ConfigureContext(_services).GetAwaiter().GetResult();
        _plugin.Configure(_provider!, host: null).GetAwaiter().GetResult();
    }

    [Fact]
    [Then("The Assemblies collection should be accessible in Install", "UAC049")]
    public void Assemblies_In_Install()
    {
        _plugin!.AssembliesAvailableInInstall.ShouldBeTrue();
    }

    [Fact]
    [Then("The Assemblies collection should be accessible in ConfigureContext", "UAC050")]
    public void Assemblies_In_ConfigureContext()
    {
        _plugin!.AssembliesAvailableInConfigureContext.ShouldBeTrue();
    }

    [Fact]
    [Then("The Assemblies collection should be accessible in Configure", "UAC051")]
    public void Assemblies_In_Configure()
    {
        _plugin!.AssembliesAvailableInConfigure.ShouldBeTrue();
    }

    [Fact]
    [Then("The collection should not be serialized to JSON", "UAC052")]
    public void Assemblies_Not_Serialized()
    {
        // Serializing plugin should not include the Assemblies collection as a property named exactly "Assemblies"
        var json = System.Text.Json.JsonSerializer.Serialize(_plugin);
        using var doc = System.Text.Json.JsonDocument.Parse(json);
        var root = doc.RootElement;
        root.TryGetProperty("Assemblies", out var _).ShouldBeFalse();
    }
}
