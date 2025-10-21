using LowlandTech.Plugins.Tests.Fixtures;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

[Scenario(
    specId: "VCHIP-0010-UC06-SC28",
    title: "Handle empty Assemblies collection",
    given: "Given a plugin has an empty Assemblies collection",
    when: "When the plugin needs to reference its assemblies",
    then: "Then the collection should be safely enumerable")]
public sealed class SC28_EmptyAssembliesCollection : WhenTestingForV2<ErrorHandlingTestFixture>
{
    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given() { }
    protected override void When() { }

    [Fact]
    [Then("The collection should be safely enumerable", "UAC086")]
    public void Collection_Enumerable()
    {
        var plugin = new TestLifecyclePlugin();
        
        // Assemblies collection is initialized as empty list in base Plugin constructor
        plugin.Assemblies.ShouldNotBeNull();
        plugin.Assemblies.ShouldBeEmpty();
        
        // Should be safe to enumerate
        Should.NotThrow(() =>
        {
            foreach (var _ in plugin.Assemblies)
            {
                // no-op
            }
        });
    }

    [Fact]
    [Then("No null reference exceptions should occur", "UAC087")]
    public void No_NullReference()
    {
        var plugin = new TestLifecyclePlugin();
        
        // Accessing empty collection should not throw
        Should.NotThrow(() =>
        {
            var count = plugin.Assemblies.Count;
            var any = plugin.Assemblies.Any();
            var first = plugin.Assemblies.FirstOrDefault();
        });
    }

    [Fact]
    [Then("The plugin should handle the empty collection gracefully", "UAC088")]
    public void Handles_Empty_Collection()
    {
        var services = new ServiceCollection();
        var plugin = new TestLifecyclePlugin();
        // Assemblies collection is empty by default
        
        Should.NotThrow(() => services.AddPlugin(plugin));
        var sp = services.BuildServiceProvider();
        sp.GetServices<IPlugin>().ShouldContain(plugin);
    }
}
