using LowlandTech.Plugins.Tests.Fixtures;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC11",
    title: "Handle null service collection",
    given: "Given a null IServiceCollection is passed to AddPlugin",
    when: "When the method attempts to register the plugin",
    then: "Then an ArgumentNullException should be thrown")]
public sealed class SC11_NullServiceCollection : WhenTestingForV2<ErrorHandlingTestFixture>
{
    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given() { }
    protected override void When() { }

    [Fact]
    [Then("An ArgumentNullException should be thrown", "UAC036")]
    public void Null_ServiceCollection_Throws()
    {
        IServiceCollection? services = null;
        var plugin = new InstallThrowingPlugin();
        Should.Throw<ArgumentNullException>(() => services!.AddPlugin(plugin));
    }
}
