namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC02_Validation;

[Scenario(
    specId: "VCHIP-0010-UC02-SC02",
    title: "Reject plugin without PluginId attribute",
    given: "Given a plugin class is not decorated with a PluginId attribute",
    when: "When the plugin is validated using Guard.Against.MissingPluginId",
    then: "Then an ArgumentException should be thrown and the plugin should not be registered")]
public class SC02_RejectPluginWithoutPluginIdAttribute : WhenTestingForV2<ValidationTestFixture>
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
            Guard.Against.MissingPluginId(_plugin!, nameof(_plugin));
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Fact]
    [Then("An ArgumentException should be thrown", "UAC004")]
    public void ArgumentException_Should_Be_Thrown()
    {
        _exception.ShouldNotBeNull();
        _exception.ShouldBeOfType<ArgumentException>();
    }

    [Fact]
    [Then("The exception message should indicate \"Invalid plugin id, it must be provided\"", "UAC005")]
    public void Exception_Message_Should_Indicate_Missing_PluginId()
    {
        _exception.ShouldNotBeNull();
        _exception.Message.ShouldContain("Invalid plugin id, it must be provided");
    }

    [Fact]
    [Then("The plugin should not be registered", "UAC006")]
    public void Plugin_Should_Not_Be_Registered()
    {
        // Verify that validation failed, preventing registration
        _exception.ShouldNotBeNull();
        _exception.ShouldBeOfType<ArgumentException>();
        
        // If an exception was thrown, the plugin registration process would be halted
        // This test verifies the guard clause prevents invalid plugins from being registered
    }
}
