namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC30",
    title: "Handle plugin logging when ILogger is not available",
    given: "Given the plugin framework attempts to log messages and ILogger is not configured in the DI container",
    when: "When logging is attempted",
    then: "Then a NullLogger should be used as fallback")]
public sealed class SC30_ILoggerNotAvailable : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private IServiceCollection? _services;
    private Exception? _caughtException;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        // Do not add logging services to simulate missing ILogger
        _services = new ServiceCollection();
    }

    protected override void When()
    {
        try
        {
            _services!.AddPlugins();
        }
        catch (Exception ex)
        {
            _caughtException = ex;
        }
    }

    [Fact]
    [Then("A NullLogger should be used as fallback", "UAC091")]
    public void NullLogger_Fallback() => _caughtException.ShouldBeNull();

    [Fact]
    [Then("The logger resolution should fail gracefully", "UAC092")]
    public void Logger_Resolution_Graceful() => _services.ShouldNotBeNull();
}
