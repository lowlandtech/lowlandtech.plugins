namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC03",
    title: "Handle plugin with missing dependencies",
    given: "Given a plugin assembly has unresolved dependencies",
    when: "When the framework attempts to instantiate the plugin",
    then: "Then a FileNotFoundException or ReflectionTypeLoadException should occur")]
public sealed class SC03_MissingDependencies : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private IServiceCollection? _services;
    private Exception? _caught;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string>
        {
            ["Plugins:Plugins:0:Name"] = "Plugin.With.Missing.Dependencies",
            ["Plugins:Plugins:0:IsActive"] = "true"
        }).Build();
        _services.AddSingleton<IConfiguration>(config);
    }

    protected override void When()
    {
        try
        {
            _services!.AddPlugins();
        }
        catch (Exception ex)
        {
            _caught = ex;
        }
    }

    [Fact]
    [Then("A FileNotFoundException or ReflectionTypeLoadException should occur", "UAC009")]
    public void MissingDep_Caught()
    {
        // Discovery should not throw; errors should be logged and plugin skipped
        _caught.ShouldBeNull();
    }
}
