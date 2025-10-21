namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC02_Validation;

[Scenario(
    specId: "VCHIP-0010-UC02-SC14",
    title: "PluginId attribute can be retrieved via reflection",
    given: "Given a plugin class BackendPlugin with PluginId attribute",
    when: "When I use Attribute.GetCustomAttribute to retrieve the PluginId",
    then: "Then the PluginId attribute should be found and accessible")]
public class SC14_ReflectionRetrievesPluginIdAttribute : WhenTestingForV2<ValidationTestFixture>
{
    private System.Attribute? _attr;

    protected override ValidationTestFixture For() => new();

    protected override void Given() { }

    protected override void When()
    {
        _attr = Attribute.GetCustomAttribute(typeof(BackendPlugin), typeof(PluginId));
    }

    [Fact]
    [Then("The PluginId attribute should be found", "UAC034")]
    public void Attribute_Should_Be_Found()
    {
        _attr.ShouldNotBeNull();
    }

    [Fact]
    [Then("The attribute type should be PluginId", "UAC035")]
    public void Attribute_Type_Should_Be_PluginId()
    {
        _attr.ShouldBeOfType<PluginId>();
    }

    [Fact]
    [Then("The Id property should be accessible", "UAC036")]
    public void Id_Property_Should_Be_Accessible()
    {
        var pid = (PluginId)_attr!;
        pid.Id.ShouldNotBeNullOrEmpty();
    }
}
