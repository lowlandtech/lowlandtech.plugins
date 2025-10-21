using System.Diagnostics;
using System.Threading;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC19",
    title: "Handle plugin that takes too long to configure",
    given: "Given a plugin Configure method has an infinite loop or very long operation",
    when: "When UsePlugins calls Configure on the plugin",
    then: "Then the application may hang waiting for the plugin")]
public sealed class SC19_LongRunningConfigure : WhenTestingForV2<ErrorHandlingTestFixture>
{
    [PluginId("ea999999-0000-4000-8000-000000000009")]
    public class SlowPlugin : Plugin
    {
        public override void Install(IServiceCollection services) { }
        public override Task Configure(IServiceProvider provider, object? host = null)
        {
            // simulate long-running task
            Thread.Sleep(5000);
            return Task.CompletedTask;
        }
    }

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given() { }
    protected override void When() { }

    [Fact]
    [Then("Long running plugin may block; test ensures it's detectable", "UAC059")]
    public void LongRunning_Detectable()
    {
        var services = new ServiceCollection();
        services.AddPlugin(new SlowPlugin());
        var sp = services.BuildServiceProvider();
        var plugins = sp.GetServices<IPlugin>();

        // Run configure with timeout
        var sw = Stopwatch.StartNew();
        foreach (var p in plugins)
        {
            p.Configure(sp, host: null).GetAwaiter().GetResult();
        }
        sw.Stop();
        sw.ElapsedMilliseconds.ShouldBeGreaterThanOrEqualTo(5000);
    }
}
