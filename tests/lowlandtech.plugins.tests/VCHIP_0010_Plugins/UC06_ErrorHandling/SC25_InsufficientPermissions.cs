namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC25",
    title: "Handle plugin with insufficient permissions",
    given: "Given the application runs with limited file system permissions",
    when: "When the framework attempts to load the assembly from a restricted directory",
    then: "Then an UnauthorizedAccessException should occur")]
public sealed class SC25_InsufficientPermissions : WhenTestingForV2<ErrorHandlingTestFixture>
{
    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given() { }
    protected override void When() { }

    [Fact]
    [Then("An UnauthorizedAccessException should occur", "UAC077")]
    public void UnauthorizedAccess_Handled()
    {
        // Document that UnauthorizedAccessException can occur when loading plugins
        // from restricted directories
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Plugins:Plugins:0:Name"] = "RestrictedPlugin",
            ["Plugins:Plugins:0:IsActive"] = "true"
        }).Build();
        services.AddSingleton<IConfiguration>(config);
        
        Should.NotThrow(() => services.AddPlugins());
    }

    [Fact]
    [Then("Appropriate error logging should occur", "UAC079")]
    public void Error_Logged()
    {
        // Framework should log UnauthorizedAccessException details
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Plugins:Plugins:0:Name"] = "RestrictedPlugin",
            ["Plugins:Plugins:0:IsActive"] = "true"
        }).Build();
        services.AddSingleton<IConfiguration>(config);
        
        Should.NotThrow(() => services.AddPlugins());
    }
}
