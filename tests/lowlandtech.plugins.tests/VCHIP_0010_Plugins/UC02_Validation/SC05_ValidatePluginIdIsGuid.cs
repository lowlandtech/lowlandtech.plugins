namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC02_Validation;

[Scenario(
    specId: "VCHIP-0010-UC02-SC05",
    title: "Validate PluginId format is a valid GUID",
    given: "Given a plugin with PluginId attribute containing \"306b92e3-2db6-45fb-99ee-9c63b090f3fc\"",
    when: "When the plugin ID is extracted",
    then: "Then the ID should be a valid GUID format and usable for plugin identification")]
public class SC05_ValidatePluginIdIsGuid : WhenTestingForV2<ValidationTestFixture>
{
    private IPlugin? _plugin;
    private string? _pluginIdString;
    private Guid _pluginIdGuid;

    protected override ValidationTestFixture For() => new();

    protected override void Given()
    {
        _plugin = new ValidPlugin();
    }

    protected override void When()
    {
        _pluginIdString = Guard.Against.MissingPluginId(_plugin!, nameof(_plugin));

        // Try parse to GUID
        Guid.TryParse(_pluginIdString, out _pluginIdGuid);
    }

    [Fact]
    [Then("The ID should be a valid GUID format", "UAC011")]
    public void PluginId_Should_Be_Valid_Guid()
    {
        _pluginIdString.ShouldNotBeNull();
        // ensure string is parsable
        Guid.TryParse(_pluginIdString, out var parsed).ShouldBeTrue();
        parsed.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    [Then("The ID should be usable for plugin identification", "UAC012")]
    public void PluginId_Should_Be_Usable()
    {
        _plugin.ShouldNotBeNull();
        _pluginIdString.ShouldNotBeNull();

        var expected = Guid.Parse("306b92e3-2db6-45fb-99ee-9c63b090f3fc");
        Guid.TryParse(_pluginIdString, out var parsed).ShouldBeTrue();
        parsed.ShouldBe(expected);
    }
}
