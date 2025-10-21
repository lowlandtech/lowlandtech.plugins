namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC17",
    title: "Handle malformed JSON in configuration",
    given: "Given the appsettings.json contains invalid JSON syntax",
    when: "When the configuration is parsed",
    then: "Then a JsonException should be thrown")]
public sealed class SC17_MalformedJson : WhenTestingForV2<ErrorHandlingTestFixture>
{
    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given() { }
    protected override void When() { }

    [Fact]
    [Then("A JsonException should be thrown when parsing", "UAC053")]
    public void JsonException_Thrown()
    {
        var invalid = "{ \"Plugins\": [ { \"Name\": \"Test\", \"IsActive\": true, } ] }"; // trailing comma
        Should.Throw<System.Text.Json.JsonException>(() => System.Text.Json.JsonDocument.Parse(invalid));
    }
}
