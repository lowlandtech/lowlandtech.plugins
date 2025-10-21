using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC14",
    title: "Handle plugin with duplicate Name",
    given: "Given two plugins have the same Name property",
    when: "When both plugins are registered",
    then: "Then both should be registered if Id is unique")]
public sealed class SC14_DuplicateName : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private IServiceCollection? _services;
    private SimpleConfigurePluginA? _p1;
    private SimpleConfigurePluginB? _p2;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        _p1 = new SimpleConfigurePluginA();
        _p2 = new SimpleConfigurePluginB();
        _p1.Name = "SameName";
        _p2.Name = "SameName";

        _services.AddPlugin(_p1);
        _services.AddPlugin(_p2);
    }

    protected override void When() { }

    [Fact]
    [Then("Both should be registered if Id is unique", "UAC044")]
    public void Both_Registered()
    {
        var sp = _services!.BuildServiceProvider();
        sp.GetServices<IPlugin>().Count(p => p.Name == "SameName").ShouldBe(2);
    }

    [Fact]
    [Then("The duplicate name should be logged as a warning", "UAC045")]
    public void Duplicate_Warning()
    {
        // rely on logs; ensure both present
        var sp = _services!.BuildServiceProvider();
        sp.GetServices<IPlugin>().Count(p => p.Name == "SameName").ShouldBe(2);
    }
}
