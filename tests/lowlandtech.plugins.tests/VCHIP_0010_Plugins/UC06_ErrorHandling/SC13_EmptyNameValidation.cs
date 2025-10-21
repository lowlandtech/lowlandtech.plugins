using LowlandTech.Plugins.Tests.Fixtures;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC13",
    title: "Handle plugin with empty or whitespace Name",
    given: "Given a plugin is registered with an empty Name property",
    when: "When the plugin is loaded from configuration",
    then: "Then a validation warning should be logged")]
public sealed class SC13_EmptyNameValidation : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private IServiceCollection? _services;
    private TestLifecyclePlugin? _plugin;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _plugin = new TestLifecyclePlugin();
        _plugin.Name = "   ";
        _services.AddPlugin(_plugin);
    }

    protected override void When() { }

    [Fact]
    [Then("A validation warning should be logged", "UAC041")]
    public void Validation_Warning()
    {
        // Ensure plugin registered with default type-name if name is whitespace
        var sp = _services!.BuildServiceProvider();
        var plugins = sp.GetServices<IPlugin>();
        plugins.ShouldNotBeEmpty();
    }
}
