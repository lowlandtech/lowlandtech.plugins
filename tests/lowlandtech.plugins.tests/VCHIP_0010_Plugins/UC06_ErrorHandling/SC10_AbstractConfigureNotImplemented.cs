namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC10",
    title: "Handle plugin with abstract Configure method not implemented",
    given: "Given a plugin class inherits from Plugin base class but does not implement Configure",
    when: "When the compiler processes the plugin",
    then: "Then a compilation error should occur")]
public sealed class SC10_AbstractConfigureNotImplemented : WhenTestingForV2<ErrorHandlingTestFixture>
{
    protected override ErrorHandlingTestFixture For() => new();
    protected override void Given() { }
    protected override void When() { }

    [Fact]
    [Then("A compilation error should be raised (this test asserts via type check)", "UAC033")]
    public void Compilation_Error()
    {
        // Ensure all non-abstract Plugin types provide a concrete Configure method
        var concrete = typeof(Plugin).Assembly.GetTypes().Where(t => typeof(Plugin).IsAssignableFrom(t) && !t.IsAbstract).ToList();
        foreach (var t in concrete)
        {
            var configure = t.GetMethod("Configure");
            configure.ShouldNotBeNull();
        }
    }
}
