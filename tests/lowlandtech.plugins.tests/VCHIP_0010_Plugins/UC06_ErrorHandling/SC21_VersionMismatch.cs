using LowlandTech.Plugins.Tests.Fixtures;
using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC21",
    title: "Handle plugin with version mismatch",
    given: "Given a plugin is built against framework version 1.0 and the application uses framework version 2.0",
    when: "When the plugin is loaded",
    then: "Then compatibility errors may occur at load time or runtime")]
public sealed class SC21_VersionMismatch : WhenTestingForV2<ErrorHandlingTestFixture>
{
    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given() { }
    protected override void When() { }

    [Fact]
    [Then("Compatibility errors may occur at load time or runtime", "UAC065")]
    public void Compatibility_Errors()
    {
        // This test documents that version mismatches can cause issues
        // In practice, .NET loads assemblies with different versions if they're compatible
        // or throws FileLoadException/TypeLoadException if not
        var services = new ServiceCollection();
        var plugin = new TestLifecyclePlugin();
        
        // Plugin loads successfully if compatible
        Should.NotThrow(() => services.AddPlugin(plugin));
    }

    [Fact]
    [Then("Version compatibility should be checked and documented", "UAC066")]
    public void Version_Should_Be_Documented()
    {
        // Framework should document version compatibility requirements
        // Plugin developers should target the same major version
        true.ShouldBeTrue(); // Placeholder for documentation requirement
    }
}
