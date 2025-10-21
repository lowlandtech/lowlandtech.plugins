using Ardalis.GuardClauses;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC02_Validation;

[Scenario(
    specId: "VCHIP-0010-UC02-SC08",
    title: "Guard clause validates parameter name correctly",
    given: "Given a plugin without PluginId",
    when: "When Guard.Against.MissingPluginId is called with parameter name \"testPlugin\"",
    then: "Then the ArgumentException should reference parameter \"testPlugin\" and the error should clearly indicate which parameter failed validation")]
public class SC08_GuardClauseValidatesParameterName : WhenTestingForV2<ValidationTestFixture>
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
            Guard.Against.MissingPluginId(_plugin!, "testPlugin");
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Fact]
    [Then("The ArgumentException should reference parameter \"testPlugin\"", "UAC018")]
    public void ArgumentException_Should_Reference_Parameter()
    {
        _exception.ShouldNotBeNull();
        _exception.ShouldBeOfType<ArgumentException>();

        var argEx = (ArgumentException)_exception!;
        argEx.ParamName.ShouldBe("testPlugin");
    }

    [Fact]
    [Then("The error should clearly indicate which parameter failed validation", "UAC019")]
    public void Error_Message_Should_Indicate_Parameter()
    {
        _exception.ShouldNotBeNull();
        _exception.Message.ShouldContain("testPlugin");
    }
}
