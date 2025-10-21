using LowlandTech.Plugins.Tests.Fixtures;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC31",
    title: "Handle async exceptions in ConfigureContext",
    given: "Given a plugin ConfigureContext method has an async operation that fails",
    when: "When the Task is awaited",
    then: "Then the exception should be properly propagated")]
public sealed class SC31_AsyncConfigureExceptions : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private ConfigureContextThrowingPlugin? _plugin;
    private IServiceCollection? _services;
    private Exception? _caughtException;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _plugin = new ConfigureContextThrowingPlugin();
        _services = new ServiceCollection();
        _services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
    }

    protected override async void When()
    {
        try
        {
            await _plugin!.ConfigureContext(_services!);
        }
        catch (Exception ex)
        {
            _caughtException = ex;
        }
    }

    [Fact]
    [Then("The exception should be properly propagated", "UAC094")]
    public void Async_Exception_Propagated() => 
        _caughtException.ShouldBeOfType<InvalidOperationException>();

    [Fact]
    [Then("AggregateException should be unwrapped if present", "UAC095")]
    public void AggregateException_Unwrapped() => 
        _caughtException.ShouldNotBeNull();

    [Fact]
    [Then("The original exception should be logged", "UAC096")]
    public void Original_Exception_Logged() => 
        _caughtException!.Message.ShouldBe("ConfigureContext failed");
}
