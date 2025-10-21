namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC27",
    title: "Handle plugin with no parameterless constructor",
    given: "Given a plugin class has a constructor requiring parameters",
    when: "When Activator.CreateInstance attempts to instantiate the plugin",
    then: "Then a MissingMethodException should be thrown")]
public sealed class SC27_NoParameterlessConstructor : WhenTestingForV2<ErrorHandlingTestFixture>
{
    [PluginId("g1111111-0000-4000-8000-000000000001")]
    public class PluginWithParameters : Plugin
    {
        public PluginWithParameters(string requiredParam)
        {
            // Constructor requires a parameter
        }

        public override void Install(IServiceCollection services) { }
        public override Task Configure(IServiceProvider provider, object? host = null) => Task.CompletedTask;
    }

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given() { }
    protected override void When() { }

    [Fact]
    [Then("A MissingMethodException should be thrown", "UAC083")]
    public void MissingMethodException_Thrown()
    {
        // Activator.CreateInstance requires a parameterless constructor
        Should.Throw<MissingMethodException>(() => 
            Activator.CreateInstance(typeof(PluginWithParameters)));
    }

    [Fact]
    [Then("The plugin should fail to load", "UAC085")]
    public void Plugin_Fails_To_Load()
    {
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Plugins:Plugins:0:Name"] = "LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling.SC27_NoParameterlessConstructor+PluginWithParameters",
            ["Plugins:Plugins:0:IsActive"] = "true"
        }).Build();
        services.AddSingleton<IConfiguration>(config);
        services.AddPlugins();
        
        var sp = services.BuildServiceProvider();
        // Plugin should not be loaded due to missing parameterless constructor
        sp.GetServices<IPlugin>().ShouldBeEmpty();
    }
}
