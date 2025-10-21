namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC03_DiscoveryAndLoading;

[Scenario(
    specId: "VCHIP-0010-UC03-SC08",
    title: "Load plugin by type registration (ASP.NET Core)",
    given: "Given I have a plugin type \"BackendPlugin\"",
    when: "When I call AddPlugin<BackendPlugin>()",
    then: "Then the plugin type should be instantiated and registered")]
public sealed class SC08_DirectRegistrationType : WhenTestingForV2<AspNetCoreTestFixture>
{
    private IServiceCollection? _services;

    protected override AspNetCoreTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _services.AddSingleton(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>()).Build());
    }

    protected override void When()
    {
        _services!.AddPlugin<BackendPlugin>();
    }

    [Fact]
    [Then("Plugin instantiated", "UAC019")]
    public void Plugin_Instantiated()
    {
        var sp = _services!.BuildServiceProvider();
        var found = sp.GetService<IPlugin>();
        found.ShouldNotBeNull();
        found.ShouldBeOfType<BackendPlugin>();
    }

    [Fact]
    [Then("Registered in service collection", "UAC020")]
    public void Registered()
    {
        var descriptor = _services!.FirstOrDefault(d => d.ServiceType == typeof(IPlugin));
        descriptor.ShouldNotBeNull();
    }

    [Fact]
    [Then("Available for DI", "UAC021")]
    public void AvailableForDI()
    {
        var sp = _services!.BuildServiceProvider();
        var p = sp.GetService<IPlugin>();
        p.ShouldNotBeNull();
    }
}
