namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC02_Validation;

[Scenario(
    specId: "VCHIP-0010-UC02-SC01",
    title: "Validate plugin has PluginId attribute",
    given: "Given a plugin class is decorated with [PluginId(\"306b92e3-2db6-45fb-99ee-9c63b090f3fc\")]",
    when: "When the plugin is validated using Guard.Against.MissingPluginId",
    then: "Then the validation should pass and the plugin ID should be extracted successfully")]
public class SC01_ValidatePluginHasPluginIdAttribute : WhenTestingForV2<ValidationTestFixture>
{
    private IPlugin? _plugin;
    private string? _pluginId;

    protected override ValidationTestFixture For() => new();

    protected override void Given()
    {
        _plugin = new ValidPlugin();
    }

    protected override void When()
    {
        _pluginId = Guard.Against.MissingPluginId(_plugin!, nameof(_plugin));
    }

    [Fact]
    [Then("Validation should pass", "UAC001")]
    public void Validation_Should_Pass()
    {
        _pluginId.ShouldNotBeNull();
    }

    [Fact]
    [Then("Plugin ID should be extracted successfully", "UAC002")]
    public void Plugin_Id_Should_Be_Extracted()
    {
        _pluginId.ShouldNotBeNull();
    }

    [Fact]
    [Then("The ID should be \"306b92e3-2db6-45fb-99ee-9c63b090f3fc\"", "UAC003")]
    public void Correct_Plugin_Id_Should_Be_Returned()
    {
        _pluginId!.ShouldBe("306b92e3-2db6-45fb-99ee-9c63b090f3fc");
    }
}
