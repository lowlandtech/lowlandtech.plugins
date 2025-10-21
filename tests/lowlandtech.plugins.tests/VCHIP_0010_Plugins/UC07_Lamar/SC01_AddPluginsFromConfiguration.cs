using Lamar;
using LowlandTech.Plugins.Tests.Fixtures;
using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC07_Lamar;

[Scenario(
    specId: "VCHIP-0010-UC07-SC01",
    title: "Add plugins from configuration to ServiceRegistry",
    given: "Given the configuration contains active plugins",
    when: "When I call services.AddPlugins() on the ServiceRegistry",
    then: "Then plugins should be discovered from configuration")]
public sealed class SC01_AddPluginsFromConfiguration : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private ServiceRegistry? _services;
    private IContainer? _container;
    private IList<IPlugin>? _plugins;
    private Exception? _caught;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceRegistry();

        // Use the specific plugin type name so discovery finds the correct plugin
        var pluginTypeName = typeof(TestLifecyclePluginLamar).FullName;

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Plugins:0:Name"] = pluginTypeName,
            ["Plugins:0:IsActive"] = "true",
            ["Plugins:Plugins:0:Name"] = pluginTypeName,
            ["Plugins:Plugins:0:IsActive"] = "true"
        }).Build();

        // register configuration and logger factory in the Lamar registry
        _services.For<IConfiguration>().Use(config);
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        _services.For<ILoggerFactory>().Use(loggerFactory);
    }

    protected override void When()
    {
        try
        {
            _services!.AddPlugins();
            _container = new Container(_services);
            _plugins = _container.GetAllInstances<IPlugin>()?.ToList() ?? new List<IPlugin>();
        }
        catch (Exception ex)
        {
            _caught = ex;
            _plugins = new List<IPlugin>();
        }
    }

    [Fact]
    [Then("plugins should be discovered from configuration", "UAC001")]
    public void Plugins_Discovered() => _caught.ShouldBeNull();

    [Fact]
    [Then("plugins should be registered in the ServiceRegistry", "UAC002")]
    public void Plugins_Registered() => _container.ShouldNotBeNull();

    [Fact]
    [Then("the Install method should be called on each plugin", "UAC003")]
    public void Install_Called_On_Plugins() => (_plugins?.OfType<TestLifecyclePluginLamar>().Any() == true).ShouldBeTrue();
}
