namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC02_Validation;

[Scenario(
    specId: "VCHIP-0010-UC02-SC17",
    title: "Guard extension method is available on IGuardClause",
    given: "Given I have an IGuardClause instance",
    when: "When I call the MissingPluginId extension method",
    then: "Then the method should be available as an extension and integrate with Ardalis.GuardClauses library")]
public class SC17_GuardExtensionAvailable : WhenTestingForV2<ValidationTestFixture>
{
    protected override ValidationTestFixture For() => new();

    protected override void Given() { }

    protected override void When() { }

    [Fact]
    [Then("The method should be available as an extension", "UAC042")]
    public void GuardExtension_Should_Be_Available()
    {
        // call extension method to ensure it compiles and runs
        var id = Ardalis.GuardClauses.Guard.Against.MissingPluginId(new ValidPlugin(), nameof(ValidPlugin));
        id.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    [Then("It should integrate with Ardalis.GuardClauses library", "UAC043")]
    public void GuardIntegration_Should_Work()
    {
        Should.NotThrow(() => Ardalis.GuardClauses.Guard.Against.MissingPluginId(new ValidPlugin(), nameof(ValidPlugin)));
    }
}
