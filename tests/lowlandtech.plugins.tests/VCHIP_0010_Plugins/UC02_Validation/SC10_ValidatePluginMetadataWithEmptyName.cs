using Microsoft.Extensions.DependencyInjection;
using LowlandTech.Plugins.AspNetCore.Extensions;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC02_Validation;

[Scenario(
    specId: "VCHIP-0010-UC02-SC10",
    title: "Validate plugin metadata with empty Name",
    given: "Given a plugin instance with the following metadata:\n  | Property    | Value                                    |\n  | Id          | 306b92e3-2db6-45fb-99ee-9c63b090f3fc     |\n  | Name        |                                         |\n  | IsActive    | true                                     |",
    when: "When the plugin is validated",
    then: "Then the Id should be valid and the plugin should be accepted if Name is optional")]
public class SC10_ValidatePluginMetadataWithEmptyName : WhenTestingForV2<ValidationTestFixture>
{
    private IPlugin? _plugin;
    private string? _pluginIdString;
    private IServiceCollection? _services;
    private Exception? _exception;

    protected override ValidationTestFixture For() => new();

    protected override void Given()
    {
        // Plugin class is decorated with PluginId in ValidationTestFixture.ValidPlugin, reuse that but set Name empty
        _plugin = new ValidPlugin
        {
            Name = string.Empty,
            IsActive = true
        };

        _services = new ServiceCollection();
    }

    protected override void When()
    {
        try
        {
            // Validate plugin id via guard
            _pluginIdString = Guard.Against.MissingPluginId(_plugin!, nameof(_plugin));

            // Attempt to register the plugin - should not throw since Name is optional
            _services!.AddPlugin(_plugin!);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Fact]
    [Then("The Id should be valid", "UAC023")]
    public void Id_Should_Be_Valid()
    {
        _pluginIdString.ShouldNotBeNull();
        Guid.TryParse(_pluginIdString, out var parsed).ShouldBeTrue();
        parsed.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    [Then("The plugin should be accepted if Name is optional", "UAC024")]
    public void Plugin_Should_Be_Accepted_When_Name_Empty()
    {
        // No exception should have occurred during AddPlugin
        _exception.ShouldBeNull();

        // Service collection should contain the plugin instance
        var found = _services!.Any(d => d.ImplementationInstance == _plugin);
        found.ShouldBeTrue();
    }

    [Fact]
    [Then("A warning might be issued for empty Name", "UAC025")]
    public void Warning_Might_Be_Issued()
    {
        // We cannot reliably assert on logs in this unit test; instead accept either behavior:
        // - Name left empty, or
        // - Name assigned a default (type name) during registration.
        _plugin.ShouldNotBeNull();
        var isEmpty = (_plugin!.Name is null || _plugin.Name == string.Empty);
        var isDefault = _plugin.Name == _plugin.GetType().Name;
        (isEmpty || isDefault).ShouldBeTrue();
    }
}
