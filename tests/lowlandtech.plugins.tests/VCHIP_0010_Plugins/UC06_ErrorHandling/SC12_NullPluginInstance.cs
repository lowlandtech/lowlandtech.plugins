namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC12",
    title: "Handle null plugin instance",
    given: "Given a null plugin instance is passed to AddPlugin",
    when: "When the guard clause checks the plugin",
    then: "Then an ArgumentNullException should be thrown")]
public sealed class SC12_NullPluginInstance : WhenTestingForV2<ErrorHandlingTestFixture>
{
    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given() { }
    protected override void When() { }

    [Fact]
    [Then("An ArgumentNullException should be thrown", "UAC038")]
    public void Null_Plugin_Throws()
    {
        var services = new ServiceCollection();
        IPlugin? plugin = null;
        Should.Throw<ArgumentNullException>(() => services.AddPlugin(plugin!));
    }
}
