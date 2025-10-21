using Microsoft.Extensions.DependencyInjection;
using LowlandTech.Plugins.AspNetCore.Extensions;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC02_Validation;

[Scenario(
    specId: "VCHIP-0010-UC02-SC11",
    title: "Validate plugin with empty Name gets default value",
    given: "Given a plugin with Name set to empty string",
    when: "When the plugin is registered",
    then: "Then a default name should be assigned and validation behavior should be consistent")]
public class SC11_ValidatePluginEmptyNameGetsDefault : WhenTestingForV2<ValidationTestFixture>
{
    private IPlugin? _plugin;
    private IServiceCollection? _services;

    protected override ValidationTestFixture For() => new();

    protected override void Given()
    {
        _plugin = new ValidPlugin { Name = string.Empty };
        _services = new ServiceCollection();
    }

    protected override void When()
    {
        // Attempt registration
        _services!.AddPlugin(_plugin!);
    }

    [Fact]
    [Then("A default name should be assigned", "UAC026")]
    public void Default_Name_Assigned()
    {
        _plugin.ShouldNotBeNull();
        // Name should be assigned to type name if empty
        _plugin.Name.ShouldNotBeNull();
        _plugin.Name.ShouldBe(_plugin.GetType().Name);
    }

    [Fact]
    [Then("Validation behavior should be consistent", "UAC027")]
    public void Validation_Behavior_Consistent()
    {
        // Plugin should be registered in services
        var found = _services!.Any(d => d.ImplementationInstance == _plugin || d.ImplementationType == _plugin!.GetType());
        found.ShouldBeTrue();
    }
}
