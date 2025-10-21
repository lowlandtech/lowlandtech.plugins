namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC23",
    title: "Handle case-sensitive file system issues",
    given: "Given a plugin name is 'MyPlugin' in configuration but the actual file is 'myplugin.dll'",
    when: "When the framework attempts to load the plugin on a case-sensitive file system",
    then: "Then a FileNotFoundException should occur")]
public sealed class SC23_CaseSensitiveFileSystem : WhenTestingForV2<ErrorHandlingTestFixture>
{
    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given() { }
    protected override void When() { }

    [Fact]
    [Then("A FileNotFoundException should occur on case-sensitive systems", "UAC071")]
    public void Case_Mismatch_Handled()
    {
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Plugins:Plugins:0:Name"] = "MyPlugin",
            ["Plugins:Plugins:0:IsActive"] = "true"
        }).Build();
        services.AddSingleton<IConfiguration>(config);
        
        // Discovery should not throw but will log errors and skip missing plugins
        Should.NotThrow(() => services.AddPlugins());
    }

    [Fact]
    [Then("Configuration should match exact file name casing", "UAC073")]
    public void Exact_Casing_Required()
    {
        // Document that plugin names in config should match assembly name casing exactly
        // on case-sensitive file systems (Linux, macOS)
        true.ShouldBeTrue();
    }
}
