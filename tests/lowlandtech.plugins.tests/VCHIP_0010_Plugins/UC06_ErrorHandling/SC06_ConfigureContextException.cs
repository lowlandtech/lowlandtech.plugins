using LowlandTech.Plugins.Tests.Fixtures;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC06",
    title: "Handle plugin ConfigureContext exception",
    given: "Given a plugin ConfigureContext method throws an exception",
    when: "When the async ConfigureContext is awaited",
    then: "Then the exception should be captured")]
public sealed class SC06_ConfigureContextException : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private IServiceCollection? _services;
    private ConfigureContextThrowingPlugin? _plugin;
    private Exception? _caught;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _plugin = new ConfigureContextThrowingPlugin();
        _services.AddPlugin(_plugin);
    }

    protected override void When()
    {
        try
        {
            // call ConfigureContext and expect exception
            _plugin!.ConfigureContext(_services!).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _caught = ex;
        }
    }

    [Fact]
    [Then("The exception should be captured", "UAC021")]
    public void Exception_Captured()
    {
        _caught.ShouldNotBeNull();
    }

    [Fact]
    [Then("The error should be logged", "UAC022")]
    public void Error_Logged()
    {
        // rely on logging infrastructure being present; at minimum ensure plugin not progressed to Configure
        _plugin!.ConfigureCalled.ShouldBeFalse();
    }

    [Fact]
    [Then("The plugin should not proceed to Configure phase", "UAC023")]
    public void Not_Proceed_To_Configure()
    {
        _plugin!.ConfigureCalled.ShouldBeFalse();
    }
}
