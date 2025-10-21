namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC02_Validation;

[Scenario(
    specId: "VCHIP-0010-UC02-SC20",
    title: "Guard clause provides helpful error messages",
    given: "Given a plugin fails MissingPluginId validation",
    when: "When the ArgumentException is thrown",
    then: "Then the error message should clearly state the problem and include parameter name and helpful guidance")]
public class SC20_GuardProvidesHelpfulMessages : WhenTestingForV2<ValidationTestFixture>
{
    private Exception? _exception;

    protected override ValidationTestFixture For() => new();

    protected override void Given()
    {
        // create invalid plugin
        var plugin = new InvalidPluginNoId();
        try
        {
            Ardalis.GuardClauses.Guard.Against.MissingPluginId(plugin, "plugin");
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    protected override void When() { }

    [Fact]
    [Then("The error message should clearly state the problem", "UAC050")]
    public void Error_Message_Should_State_Problem()
    {
        _exception.ShouldNotBeNull();
        _exception.Message.ShouldContain("Invalid plugin id");
    }

    [Fact]
    [Then("The error should include the parameter name", "UAC051")]
    public void Error_Should_Include_Parameter_Name()
    {
        var arg = (ArgumentException)_exception!;
        arg.ParamName.ShouldBe("plugin");
    }

    [Fact]
    [Then("The error should help developers fix the issue quickly", "UAC052")]
    public void Error_Should_Help_Dev()
    {
        _exception.Message.ShouldContain("provide");
    }
}
