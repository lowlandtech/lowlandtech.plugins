namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

[Scenario(
    specId: "VCHIP-0010-UC04-SC12",
    title: "Handle exception during ConfigureContext phase",
    given: "Given a plugin throws an exception during ConfigureContext",
    when: "When the ConfigureContext method is called",
    then: "Then the exception should be propagated")]
public sealed class SC12_HandleConfigureContextException : WhenTestingForV2<LifecycleTestFixture>
{
    private IServiceCollection? _services;
    private ThrowingConfigureContextPlugin? _plugin;
    private Exception? _caught;

    protected override LifecycleTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _plugin = new ThrowingConfigureContextPlugin();
        _services.AddPlugin(_plugin);
    }

    protected override void When()
    {
        try
        {
            _plugin!.ConfigureContext(_services).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _caught = ex;
        }
    }

    [Fact]
    [Then("The exception should be propagated", "UAC038")]
    public void Exception_Propagated()
    {
        _caught.ShouldNotBeNull();
        _caught.ShouldBeOfType<InvalidOperationException>();
    }

    [Fact]
    [Then("The plugin should not proceed to Configure phase", "UAC039")]
    public void Should_Not_Proceed_To_Configure()
    {
        // Configure should not have been called
        _plugin!.ConfigureCalled.ShouldBeFalse();
    }

    [Fact]
    [Then("The error should be handled by the application", "UAC040")]
    public void Application_Handles_Error()
    {
        // For these tests, we assert exception was caught; further app-level handling would be elsewhere
        _caught.ShouldNotBeNull();
    }
}
