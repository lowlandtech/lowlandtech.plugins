using Microsoft.Extensions.DependencyInjection;
using LowlandTech.Plugins.AspNetCore.Extensions;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC02_Validation;

[Scenario(
    specId: "VCHIP-0010-UC02-SC09",
    title: "Validate plugin instance during AddPlugin",
    given: "Given I attempt to add a plugin using AddPlugin method\nAnd the plugin is missing the PluginId attribute",
    when: "When the AddPlugin method executes guard validation",
    then: "Then the plugin should be rejected before registration and an ArgumentException should be thrown and the service collection should not contain the invalid plugin")]
public class SC09_ValidatePluginInstanceDuringAddPlugin : WhenTestingForV2<ValidationTestFixture>
{
    private IServiceCollection? _services;
    private IPlugin? _plugin;
    private Exception? _exception;

    protected override ValidationTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _plugin = new InvalidPluginNoId();
    }

    protected override void When()
    {
        try
        {
            // Attempt to add plugin which lacks PluginId attribute
            _services!.AddPlugin(_plugin!);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Fact]
    [Then("The plugin should be rejected before registration", "UAC020")]
    public void Plugin_Should_Be_Rejected()
    {
        _exception.ShouldNotBeNull();
        _exception.ShouldBeOfType<ArgumentException>();
    }

    [Fact]
    [Then("An ArgumentException should be thrown", "UAC021")]
    public void ArgumentException_Should_Be_Thrown()
    {
        _exception.ShouldNotBeNull();
        _exception.ShouldBeOfType<ArgumentException>();
    }

    [Fact]
    [Then("The service collection should not contain the invalid plugin", "UAC022")]
    public void ServiceCollection_Should_Not_Contain_Invalid_Plugin()
    {
        _services.ShouldNotBeNull();
        // No descriptors for IPlugin should reference the invalid plugin type or instance
        var hasInvalidInstance = _services.Any(d => d.ImplementationInstance == _plugin);
        var hasInvalidType = _services.Any(d => d.ImplementationType == _plugin!.GetType());

        (hasInvalidInstance || hasInvalidType).ShouldBeFalse();
    }
}
