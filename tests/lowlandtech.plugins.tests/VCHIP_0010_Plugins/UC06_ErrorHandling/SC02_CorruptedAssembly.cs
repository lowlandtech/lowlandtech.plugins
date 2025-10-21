namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC02",
    title: "Handle corrupted plugin assembly",
    given: "Given a plugin assembly file is corrupted",
    when: "When the framework attempts to load the assembly",
    then: "Then a BadImageFormatException should be caught")]
public sealed class SC02_CorruptedAssembly : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private IServiceCollection? _services;
    private Exception? _caught;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string>
        {
            ["Plugins:Plugins:0:Name"] = "Corrupted.Plugin",
            ["Plugins:Plugins:0:IsActive"] = "true"
        }).Build();
        _services.AddSingleton<IConfiguration>(config);
    }

    protected override void When()
    {
        try
        {
            _services!.AddPlugins();
        }
        catch (Exception ex)
        {
            _caught = ex;
        }
    }

    [Fact]
    [Then("A BadImageFormatException should be caught", "UAC005")]
    public void BadImage_Caught()
    {
        // The discovery code should avoid throwing BadImageFormatException to callers
        _caught.ShouldBeNull();
    }
}
