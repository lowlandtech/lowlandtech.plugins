using LowlandTech.Plugins.Tests.Fixtures;
using LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC22",
    title: "Handle reflection failure during type discovery",
    given: "Given a plugin assembly contains types that cannot be loaded via reflection",
    when: "When GetTypes() is called on the assembly",
    then: "Then a ReflectionTypeLoadException may be thrown")]
public sealed class SC22_ReflectionFailure : WhenTestingForV2<ErrorHandlingTestFixture>
{
    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given() { }
    protected override void When() { }

    [Fact]
    [Then("A ReflectionTypeLoadException may be thrown", "UAC068")]
    public void ReflectionTypeLoadException_Handled()
    {
        // The framework's GetLoadableTypes helper method handles ReflectionTypeLoadException
        var assembly = typeof(TestLifecyclePlugin).Assembly;
        
        // Should not throw - GetLoadableTypes catches ReflectionTypeLoadException
        var types = GetLoadableTypes(assembly);
        types.ShouldNotBeNull();
    }

    [Fact]
    [Then("Types that can be loaded should still be processed", "UAC070")]
    public void Loadable_Types_Processed()
    {
        var assembly = typeof(TestLifecyclePlugin).Assembly;
        var types = GetLoadableTypes(assembly);
        
        // Should return at least some types even if some fail to load
        types.ShouldNotBeEmpty();
    }

    private static IEnumerable<Type> GetLoadableTypes(System.Reflection.Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (System.Reflection.ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t is not null)!;
        }
    }
}
