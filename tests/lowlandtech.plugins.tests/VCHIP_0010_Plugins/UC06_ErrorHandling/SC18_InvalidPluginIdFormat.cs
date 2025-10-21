namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC18",
    title: "Handle invalid PluginId GUID format",
    given: "Given a PluginId attribute contains an invalid GUID string",
    when: "When the plugin is loaded",
    then: "Then the Id should be treated as a string")]
public sealed class SC18_InvalidPluginIdFormat : WhenTestingForV2<ErrorHandlingTestFixture>
{
    [PluginId("not-a-guid")]
    public class BadIdPlugin : Plugin
    {
        public override void Install(IServiceCollection services) { }
        public override Task Configure(IServiceProvider provider, object? host = null) => Task.CompletedTask;
    }

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given() { }
    protected override void When() { }

    [Fact]
    [Then("The Id should be treated as a string or empty", "UAC056")]
    public void Invalid_Id_Handled()
    {
        var p = new BadIdPlugin();
        // Plugin constructor tries to parse GUID; expect Id to be Guid.Empty when invalid
        p.Id.ShouldBe(Guid.Empty);
    }
}
