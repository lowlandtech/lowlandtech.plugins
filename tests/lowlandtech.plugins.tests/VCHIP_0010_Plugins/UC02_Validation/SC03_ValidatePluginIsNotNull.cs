using Ardalis.GuardClauses;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC02_Validation;

[Scenario(
    specId: "VCHIP-0010-UC02-SC03",
    title: "Validate plugin is not null",
    given: "Given a null plugin reference",
    when: "When Guard.Against.MissingPluginId is called with the null plugin",
    then: "Then an ArgumentNullException should be thrown for the plugin parameter")]
public class SC03_ValidatePluginIsNotNull : WhenTestingForV2<ValidationTestFixture>
{
    private IPlugin? _plugin;
    private Exception? _exception;

    protected override ValidationTestFixture For() => new();

    protected override void Given()
    {
        _plugin = null;
    }

    protected override void When()
    {
        try
        {
            // Use a specific parameter name to verify the ArgumentNullException refers to it
            Guard.Against.MissingPluginId(_plugin!, "testPlugin");
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Fact]
    [Then("An ArgumentNullException should be thrown for the plugin parameter", "UAC007")]
    public void ArgumentNullException_Should_Be_Thrown()
    {
        _exception.ShouldNotBeNull();
        _exception.ShouldBeOfType<ArgumentNullException>();
    }

    [Fact]
    [Then("The validation should fail before checking the PluginId attribute", "UAC008")]
    public void Validation_Fails_Before_PluginId_Check()
    {
        // Ensure the exception is ArgumentNullException (not ArgumentException from missing PluginId attribute)
        _exception.ShouldNotBeNull();
        _exception.ShouldBeOfType<ArgumentNullException>();

        var argNull = (ArgumentNullException)_exception!;
        argNull.ParamName.ShouldBe("testPlugin");
    }
}
