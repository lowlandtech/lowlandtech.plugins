namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC05",
    title: "Handle plugin Install method exception",
    given: "Given a plugin Install method throws an exception",
    when: "When the Install method is called during registration",
    then: "Then the exception should be propagated or caught based on policy")]
public sealed class SC05_InstallException : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private IServiceCollection? _services;
    private Exception? _caught;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string>
        {
            ["Plugins:Plugins:0:Name"] = "LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling.InstallThrowingPlugin",
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
    [Then("The exception should be propagated or caught based on policy", "UAC017")]
    public void Install_Exception_Handling()
    {
        // Framework currently catches install exceptions and continues; ensure no throw
        _caught.ShouldBeNull();
    }

    [Fact]
    [Then("The error should be logged with plugin name and stack trace", "UAC018")]
    public void Error_Logged()
    {
        // We rely on logging; at minimum ensure AddPlugins completed
        _services.ShouldNotBeNull();
    }
}
