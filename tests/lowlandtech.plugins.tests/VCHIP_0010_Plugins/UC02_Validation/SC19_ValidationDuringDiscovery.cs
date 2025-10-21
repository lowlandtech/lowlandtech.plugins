using System.Reflection;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC02_Validation;

[Scenario(
    specId: "VCHIP-0010-UC02-SC19",
    title: "Validation during plugin type discovery",
    given: "Given plugin discovery is scanning an assembly",
    when: "When the validation runs during discovery",
    then: "Then invalid types should be skipped and errors should be logged and other valid plugins should still be loaded")]
public class SC19_ValidationDuringDiscovery : WhenTestingForV2<ValidationTestFixture>
{
    private List<Type> _types = new();
    private List<Type> _loaded = new();
    private List<string> _errors = new();

    protected override ValidationTestFixture For() => new();

    protected override void Given()
    {
        // Simulate assembly scanning with types that include a valid and an invalid plugin
        _types.Add(typeof(BackendPlugin));
        _types.Add(typeof(InvalidPluginNoId));
    }

    protected override void When()
    {
        foreach (var t in _types)
        {
            try
            {
                if (typeof(IPlugin).IsAssignableFrom(t))
                {
                    var attr = Attribute.GetCustomAttribute(t, typeof(PluginId));
                    if (attr is null)
                    {
                        _errors.Add($"Type {t.FullName} missing PluginId");
                        continue; // skip invalid type
                    }

                    _loaded.Add(t);
                }
            }
            catch (Exception ex)
            {
                _errors.Add(ex.Message);
            }
        }
    }

    [Fact]
    [Then("The invalid type should be skipped", "UAC047")]
    public void Invalid_Type_Should_Be_Skipped()
    {
        _loaded.ShouldNotContain(typeof(InvalidPluginNoId));
    }

    [Fact]
    [Then("An error should be logged", "UAC048")]
    public void Error_Should_Be_Logged()
    {
        _errors.Count.ShouldBeGreaterThanOrEqualTo(1);
        _errors.ShouldContain(msg => msg.Contains("missing PluginId"));
    }

    [Fact]
    [Then("Other valid plugins in the assembly should still be loaded", "UAC049")]
    public void Other_Valid_Plugins_Loaded()
    {
        _loaded.ShouldContain(typeof(BackendPlugin));
    }
}
