namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC02_Validation;

[Scenario(
    specId: "VCHIP-0010-UC02-SC18",
    title: "Validate plugin with all required attributes and implementations",
    given: "Given a fully compliant plugin:",
    when: "When the plugin goes through all validations",
    then: "Then all validations should pass and the plugin should be successfully registered and ready for use")]
public class SC18_ValidateFullyCompliantPlugin : WhenTestingForV2<ValidationTestFixture>
{
    private IPlugin? _plugin;
    private IServiceCollection? _services;

    protected override ValidationTestFixture For() => new();

    protected override void Given()
    {
        _plugin = new ValidPlugin();
        _services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
    }

    protected override void When()
    {
        // Validate id
        var id = Ardalis.GuardClauses.Guard.Against.MissingPluginId(_plugin!, nameof(_plugin));

        // Attempt registration
        _services!.AddPlugin(_plugin!);
    }

    [Fact]
    [Then("All validations should pass", "UAC044")]
    public void Validations_Should_Pass()
    {
        // If registration succeeded, ensure plugin is in service collection
        var found = _services!.Any(d => d.ImplementationInstance == _plugin || d.ImplementationType == _plugin!.GetType());
        found.ShouldBeTrue();
    }

    [Fact]
    [Then("The plugin should be successfully registered", "UAC045")]
    public void Plugin_Should_Be_Registered()
    {
        var found = _services!.Any(d => d.ImplementationInstance == _plugin || d.ImplementationType == _plugin!.GetType());
        found.ShouldBeTrue();
    }

    [Fact]
    [Then("The plugin should be ready for use", "UAC046")]
    public void Plugin_Should_Be_Ready()
    {
        // Ensure plugin Configure can be called without error
        var task = _plugin!.Configure(new Microsoft.Extensions.DependencyInjection.ServiceCollection().BuildServiceProvider(), host: null);
        task.IsCompleted.ShouldBeTrue();
    }
}
