namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC04",
    title: "Handle plugin constructor exception",
    given: "Given a plugin constructor throws an exception",
    when: "When Activator.CreateInstance attempts to create the plugin",
    then: "Then the inner exception should be captured")]
public sealed class SC04_ConstructorException : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private IServiceCollection? _services;
    private Exception? _caught;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string>
        {
            ["Plugins:Plugins:0:Name"] = "LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling.ConstructorThrowingPlugin",
            ["Plugins:Plugins:0:IsActive"] = "true"
        }).Build();
        _services.AddSingleton<IConfiguration>(config);
        _services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
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
    [Then("The inner exception should be captured", "UAC013")]
    public void Inner_Exception_Captured()
    {
        // AddPlugins should catch constructor exceptions and not propagate
        _caught.ShouldBeNull();
    }

    [Fact]
    [Then("The plugin should not be registered", "UAC015")]
    public void Not_Registered()
    {
        var sp = _services!.BuildServiceProvider();
        sp.GetServices<IPlugin>().Any(p => p.GetType().Name.Contains("ConstructorThrowingPlugin")).ShouldBeFalse();
    }
}
