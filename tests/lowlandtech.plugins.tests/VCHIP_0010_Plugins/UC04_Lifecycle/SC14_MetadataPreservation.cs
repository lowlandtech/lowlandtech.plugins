using LowlandTech.Plugins.Tests.Fixtures;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

[Scenario(
    specId: "VCHIP-0010-UC04-SC14",
    title: "Plugin metadata is preserved throughout lifecycle",
    given: "Given a plugin has metadata properties set",
    when: "When the plugin goes through all lifecycle phases",
    then: "Then all metadata properties should remain unchanged")]
public sealed class SC14_MetadataPreservation : WhenTestingForV2<LifecycleTestFixture>
{
    private IServiceCollection? _services;
    private MetadataPlugin? _plugin;
    private IServiceProvider? _provider;

    protected override LifecycleTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _plugin = new MetadataPlugin();
        _services.AddPlugin(_plugin);
        _provider = _services.BuildServiceProvider();
    }

    protected override void When()
    {
        _plugin!.ConfigureContext(_services!).GetAwaiter().GetResult();
        _plugin.Configure(_provider!, host: null).GetAwaiter().GetResult();
    }

    [Fact]
    [Then("All metadata properties should remain unchanged", "UAC044")]
    public void Metadata_Remains()
    {
        _plugin!.Id.ShouldBe(Guid.Parse("306b92e3-2db6-45fb-99ee-9c63b090f3fc"));
        _plugin.Name.ShouldBe("BackendPlugin");
        _plugin.IsActive.ShouldBeTrue();
        _plugin.Description.ShouldBe("Sample backend plugin");
        _plugin.Company.ShouldBe("LowlandTech");
        _plugin.Version.ShouldBe("1.0.0");
    }

    [Fact]
    [Then("The metadata should be accessible at any lifecycle phase", "UAC045")]
    public void Metadata_Accessible()
    {
        // accessible during Configure
        _plugin!.Name.ShouldNotBeNull();
        _plugin.Id.ShouldNotBe(Guid.Empty);
    }
}
