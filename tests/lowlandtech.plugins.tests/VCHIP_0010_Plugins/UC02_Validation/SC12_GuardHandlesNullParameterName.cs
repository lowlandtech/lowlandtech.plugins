using Ardalis.GuardClauses;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC02_Validation;

[Scenario(
    specId: "VCHIP-0010-UC02-SC12",
    title: "Guard clause handles null parameter name",
    given: "Given a plugin without PluginId",
    when: "When Guard.Against.MissingPluginId is called with null parameter name",
    then: "Then an appropriate exception should be thrown and the guard should handle the null parameter name gracefully")]
public class SC12_GuardHandlesNullParameterName : WhenTestingForV2<ValidationTestFixture>
{
    private IPlugin? _plugin;
    private Exception? _exception;

    protected override ValidationTestFixture For() => new();

    protected override void Given()
    {
        _plugin = new InvalidPluginNoId();
    }

    protected override void When()
    {
        try
        {
            // Pass null for parameter name
            Guard.Against.MissingPluginId(_plugin!, null!);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Fact]
    [Then("An appropriate exception should be thrown", "UAC028")]
    public void Appropriate_Exception_Thrown()
    {
        _exception.ShouldNotBeNull();
        _exception.ShouldBeOfType<ArgumentException>();
    }

    [Fact]
    [Then("The guard should handle the null parameter name gracefully", "UAC029")]
    public void Guard_Handles_Null_Parameter_Name()
    {
        // When parameterName is null, the thrown ArgumentException should still provide a message
        _exception.ShouldNotBeNull();
        _exception.Message.ShouldContain("Invalid plugin id");
    }
}
