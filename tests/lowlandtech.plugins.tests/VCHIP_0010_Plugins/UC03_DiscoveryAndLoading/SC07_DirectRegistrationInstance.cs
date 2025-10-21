namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC03_DiscoveryAndLoading;

[Scenario(
    specId: "VCHIP-0010-UC03-SC07",
    title: "Load plugin by direct registration (instance)",
    given: "Given I have a plugin instance of type \"BackendPlugin\"",
    when: "When I call AddPlugin with the plugin instance",
    then: "Then the plugin should be registered immediately")]
public sealed class SC07_DirectRegistrationInstance : WhenTestingForV2<AspNetCoreTestFixture>
{
    private IServiceCollection? _services;
    private List<IPlugin>? _registered;

    protected override AspNetCoreTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _services.AddSingleton(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>()).Build());
    }

    protected override void When()
    {
        var plugin = new BackendPlugin { Name = "BackendPlugin", IsActive = true };
        _services!.AddPlugin(plugin);
        var sp = _services.BuildServiceProvider();
        _registered = sp.GetServices<IPlugin>().ToList();
    }

    [Fact]
    [Then("Plugin registered immediately", "UAC017")]
    public void Registered_Immediately()
    {
        _registered.ShouldNotBeNull();
        _registered.ShouldContain(p => p.GetType() == typeof(BackendPlugin));
    }

    [Fact]
    [Then("Available for DI", "UAC018")]
    public void Available_For_DI()
    {
        var sp = _services!.BuildServiceProvider();
        var found = sp.GetService<IPlugin>();
        found.ShouldNotBeNull();
    }
}
