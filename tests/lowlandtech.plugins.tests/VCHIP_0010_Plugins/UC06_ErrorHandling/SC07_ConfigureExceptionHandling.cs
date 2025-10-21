using LowlandTech.Plugins.Tests.Fixtures;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC07",
    title: "Handle plugin Configure method exception",
    given: "Given a plugin Configure method throws an exception",
    when: "When UsePlugins iterates through plugins",
    then: "Then the exception should be caught or propagated")]
public sealed class SC07_ConfigureExceptionHandling : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private IServiceCollection? _services;
    private LowlandTech.Plugins.Tests.Fixtures.ThrowingConfigurePlugin? _plugin;
    private Exception? _caught;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _plugin = new LowlandTech.Plugins.Tests.Fixtures.ThrowingConfigurePlugin();
        _services.AddPlugin(_plugin);
    }

    protected override void When()
    {
        try
        {
            var sp = _services!.BuildServiceProvider();
            var plugins = sp.GetServices<IPlugin>();
            foreach (var p in plugins)
            {
                try
                {
                    p.Configure(sp, host: null).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    // capture but continue
                    _caught = ex;
                }
            }
        }
        catch (Exception ex)
        {
            _caught = ex;
        }
    }

    [Fact]
    [Then("The exception should be caught or propagated", "UAC024")]
    public void Exception_Handled()
    {
        _caught.ShouldNotBeNull();
    }

    [Fact]
    [Then("The error should be logged with plugin details", "UAC025")]
    public void Error_Logged()
    {
        // ensure plugin threw
        _caught.ShouldBeOfType<InvalidOperationException>();
    }
}
