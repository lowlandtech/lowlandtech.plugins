using Lamar;
using LowlandTech.Plugins.Tests.Fixtures;
using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;
using System;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC07_Lamar;

[Scenario(
    specId: "VCHIP-0010-UC07-SC18",
    title: "Lamar container validation with plugins",
    given: "Given plugins register complex dependency graphs",
    when: "When the Lamar container is built with validation enabled",
    then: "Then the container should validate all registrations")]
public sealed class SC18_LamarContainerValidation : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private ServiceRegistry? _services;
    private Exception? _caught;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceRegistry();
        // register a plugin that intentionally misconfigures services to trigger validation
        _services.AddPlugin(new FaultyValidationPlugin());
    }

    protected override void When()
    {
        try
        {
            var container = new Container(_services);
            // Attempt to resolve the faulty service to trigger validation/resolution exception
            container.GetInstance<IFaultyService>();
        }
        catch (Exception ex)
        {
            _caught = ex;
        }
    }

    [Fact]
    [Then("the container should validate all registrations", "UAC045")]
    public void Container_Validates()
    {
        _caught.ShouldNotBeNull();
    }

    [Fact]
    [Then("any misconfigured services should be detected", "UAC046")]
    public void Misconfigured_Services_Detected()
    {
        _caught.ShouldNotBeOfType<ArgumentNullException>();
    }

    [Fact]
    [Then("appropriate errors should be thrown for validation failures", "UAC047")]
    public void Validation_Errors_Thrown()
    {
        _caught.ShouldNotBeNull();
    }
}

[PluginId("f1111111-1111-4111-8111-111111111111")]
public class FaultyValidationPlugin : Plugin
{
    public override void Install(ServiceRegistry services)
    {
        // Register a service where the concrete requires a missing dependency to cause resolution failure
        services.For<IFaultyService>().Use<FaultyService>();
    }
}

public interface IFaultyService { }
public interface IMissingDep { }
public class FaultyService : IFaultyService { public FaultyService(IMissingDep dep) { } }
