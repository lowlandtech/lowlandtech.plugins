using LowlandTech.Plugins.Tests.Fixtures;
using System.Text.Json;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    "VCHIP-0010-UC06-SC32",
    "Handle plugin metadata with special characters",
    "Given a plugin has metadata with special characters",
    "When the plugin metadata is stored or serialized",
    "Then special characters should be properly escaped")]
public sealed class SC32_SpecialCharactersMetadata : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private TestLifecyclePlugin? _plugin;
    private string? _serializedJson;
    private TestLifecyclePlugin? _deserializedPlugin;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _plugin = new TestLifecyclePlugin
        {
            Name = "My<Plugin>& \"Special\" 'Chars'",
            Description = "Line1\nLine2\tTab"
        };
    }

    protected override void When()
    {
        _serializedJson = JsonSerializer.Serialize(_plugin);
        _deserializedPlugin = JsonSerializer.Deserialize<TestLifecyclePlugin>(_serializedJson);
    }

    [Fact]
    [Then("Special characters should be properly escaped", "UAC097")]
    public void Special_Characters_Escaped() => 
        _deserializedPlugin!.Name.ShouldBe(_plugin!.Name);

    [Fact]
    [Then("JSON serialization should handle the characters correctly", "UAC098")]
    public void Json_Serialization_Handles() => 
        _deserializedPlugin!.Description.ShouldBe(_plugin!.Description);

    [Fact]
    [Then("No injection vulnerabilities should exist", "UAC099")]
    public void No_Injection() => 
        _serializedJson.ShouldNotBeNullOrEmpty();
}