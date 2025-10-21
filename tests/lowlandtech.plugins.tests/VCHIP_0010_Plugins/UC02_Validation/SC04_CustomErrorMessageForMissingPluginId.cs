using Ardalis.GuardClauses;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC02_Validation;

[Scenario(
    specId: "VCHIP-0010-UC02-SC04",
    title: "Custom error message for missing PluginId",
    given: "Given a plugin without a PluginId attribute",
    when: "When Guard.Against.MissingPluginId is called with a custom message \"Plugin requires unique identifier\"",
    then: "Then an ArgumentException should be thrown")]
public class SC04_CustomErrorMessageForMissingPluginId : WhenTestingForV2<ValidationTestFixture>
{
    private IPlugin? _plugin;
    private Exception? _exception;
    private const string CustomMessage = "Plugin requires unique identifier";

    protected override ValidationTestFixture For() => new();

    protected override void Given()
    {
        _plugin = new InvalidPluginNoId();
    }

    protected override void When()
    {
        try
        {
            Guard.Against.MissingPluginId(_plugin!, nameof(_plugin), CustomMessage);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Fact]
    [Then("An ArgumentException should be thrown", "UAC009")]
    public void ArgumentException_Should_Be_Thrown()
    {
        _exception.ShouldNotBeNull();
        _exception.ShouldBeOfType<ArgumentException>();
    }

    [Fact]
    [Then("The exception message should be \"Plugin requires unique identifier\"", "UAC010")]
    public void Exception_Message_Should_Match_Custom_Message()
    {
        _exception.ShouldNotBeNull();
        _exception.Message.ShouldBe(CustomMessage);
    }
}
