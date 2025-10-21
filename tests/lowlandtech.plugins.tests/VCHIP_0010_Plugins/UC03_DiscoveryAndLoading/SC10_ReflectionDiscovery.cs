namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC03_DiscoveryAndLoading;

[Scenario(
    specId: "VCHIP-0010-UC03-SC10",
    title: "Discover plugin type using reflection",
    given: "Given a plugin assembly contains a type implementing IPlugin and decorated with PluginId",
    when: "When the type discovery process scans the assembly",
    then: "Then the plugin type should be identified and instantiated")]
public sealed class SC10_ReflectionDiscovery : WhenTestingForV2<AspNetCoreTestFixture>
{
    protected override AspNetCoreTestFixture For() => new();

    [Fact]
    [Then("Type identified via reflection and instance created", "UAC024")]
    public void Reflection_Type_Discovery()
    {
        var assembly = typeof(BackendPlugin).Assembly;
        var pluginType = assembly.GetTypes().FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t));
        pluginType.ShouldNotBeNull();

        // attribute present
        var attr = Attribute.GetCustomAttribute(pluginType!, typeof(PluginId)) as PluginId;
        attr.ShouldNotBeNull();

        // create via Activator
        var instance = (IPlugin)Activator.CreateInstance(pluginType!)!;
        instance.ShouldNotBeNull();
    }

    [Fact]
    [Then("Activator created instance", "UAC025")]
    public void Activator_Created_Instance()
    {
        var assembly = typeof(BackendPlugin).Assembly;
        var pluginType = assembly.GetTypes().First(t => typeof(IPlugin).IsAssignableFrom(t));
        var instance = (IPlugin)Activator.CreateInstance(pluginType)!;
        instance.ShouldNotBeNull();
    }
}
