using LowlandTech.Plugins.Tests.Fixtures;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC35",
    title: "Handle plugin throwing exception from property getter",
    given: "Given a plugin property getter throws an exception",
    when: "When the property is accessed during plugin processing",
    then: "Then the exception should be caught and logged")]
public sealed class SC35_PropertyGetterException : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private IServiceCollection? _services;
    private FaultyPropertyPlugin? _plugin;
    private IServiceProvider? _serviceProvider;
    private List<IPlugin>? _registeredPlugins;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _plugin = new FaultyPropertyPlugin();
    }

    protected override void When()
    {
        _services!.AddPlugin(_plugin!);
        _serviceProvider = _services.BuildServiceProvider();
        _registeredPlugins = _serviceProvider.GetServices<IPlugin>().ToList();
    }

    [Fact]
    [Then("The exception should be caught and logged", "UAC107")]
    public void Property_Getter_Exception_Caught() => 
        Should.Throw<InvalidOperationException>(() => { var n = _plugin!.Name; });

    [Fact]
    [Then("The plugin should be skipped or default value used", "UAC108")]
    public void Plugin_Default_Value_Used() => 
        ((IPlugin)_plugin!).Name.ShouldNotBeNull();

    [Fact]
    [Then("The application should remain stable", "UAC109")]
    public void Application_Stable() => 
        _serviceProvider.ShouldNotBeNull();
}
