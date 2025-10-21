namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC02_Validation;

[Scenario(
    specId: "VCHIP-0010-UC02-SC15",
    title: "Validate plugin inheritance from Plugin base class",
    given: "Given a custom plugin class inherits from Plugin base class",
    when: "When the plugin is instantiated",
    then: "Then the plugin should implement IPlugin interface and all required properties/methods should be present")]
public class SC15_ValidatePluginInheritanceAndImplementation : WhenTestingForV2<ValidationTestFixture>
{
    private DerivedPlugin? _plugin;

    protected override ValidationTestFixture For() => new();

    protected override void Given()
    {
        _plugin = new DerivedPlugin();
    }

    protected override void When()
    {
        // instantiate done in Given
    }

    [Fact]
    [Then("The plugin should implement IPlugin interface", "UAC037")]
    public void Implements_IPlugin()
    {
        _plugin.ShouldNotBeNull();
        (_plugin is IPlugin).ShouldBeTrue();
    }

    [Fact]
    [Then("All required properties should be available", "UAC038")]
    public void Required_Properties_Available()
    {
        _plugin!.Name = "Test";
        _plugin.Id.ShouldNotBe(Guid.Empty);
        // check other properties exist (not null by contract)
        _plugin.Description.ShouldNotBeNull();
    }

    [Fact]
    [Then("All abstract methods should be implemented", "UAC039")]
    public void Methods_Implemented()
    {
        // Should be able to call Install and Configure
        _plugin!.Install(new Microsoft.Extensions.DependencyInjection.ServiceCollection());
        var serviceProvider = new Microsoft.Extensions.DependencyInjection.ServiceCollection().BuildServiceProvider();
        var t = _plugin.Configure(serviceProvider);
        t.IsCompletedSuccessfully.ShouldBeTrue();
    }
}

[PluginId("123e4567-e89b-12d3-a456-426614174000")]
public class DerivedPlugin : Plugin
{
    public DerivedPlugin()
    {
        // ensure Id is set by base or attribute logic
    }

    public override void Install(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
    {
        // minimal install implementation
    }

    public override Task Configure(System.IServiceProvider container, object? host = null)
    {
        return Task.CompletedTask;
    }
}
