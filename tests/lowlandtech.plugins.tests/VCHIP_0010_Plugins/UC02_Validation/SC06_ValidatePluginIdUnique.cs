using Ardalis.GuardClauses;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC02_Validation;

[Scenario(
    specId: "VCHIP-0010-UC02-SC06",
    title: "Validate PluginId attribute is unique across plugins",
    given: "Given multiple plugins are registered:",
    when: "When the plugins are validated",
    then: "Then each plugin should have a unique ID and no ID conflicts should exist")]
public class SC06_ValidatePluginIdUnique : WhenTestingForV2<ValidationTestFixture>
{
    private IPlugin? _backendPlugin;
    private IPlugin? _frontendPlugin;
    private string? _backendId;
    private string? _frontendId;

    protected override ValidationTestFixture For() => new();

    protected override void Given()
    {
        _backendPlugin = new BackendPlugin();
        _frontendPlugin = new FrontendPlugin();
    }

    protected override void When()
    {
        _backendId = Guard.Against.MissingPluginId(_backendPlugin!, nameof(_backendPlugin));
        _frontendId = Guard.Against.MissingPluginId(_frontendPlugin!, nameof(_frontendPlugin));
    }

    [Fact]
    [Then("Each plugin should have a unique ID", "UAC013")]
    public void Each_Plugin_Should_Have_Unique_Id()
    {
        _backendId.ShouldNotBeNull();
        _frontendId.ShouldNotBeNull();
        _backendId!.ShouldNotBe(_frontendId!);
    }

    [Fact]
    [Then("No ID conflicts should exist", "UAC014")]
    public void No_Id_Conflicts()
    {
        // Both IDs should be valid GUIDs and different
        Guid.TryParse(_backendId, out var b).ShouldBeTrue();
        Guid.TryParse(_frontendId, out var f).ShouldBeTrue();
        b.ShouldNotBe(Guid.Empty);
        f.ShouldNotBe(Guid.Empty);
        b.ShouldNotBe(f);
    }
}

[PluginId("306b92e3-2db6-45fb-99ee-9c63b090f3fc")]
public class BackendPlugin : Plugin
{
    public override Task Configure(IServiceProvider provider, object? host = null) => Task.CompletedTask;
    public override void Install(IServiceCollection services) { }
}

[PluginId("4a8c6f2e-1b3d-4e5f-9a7c-8d6e5f4a3b2c")]
public class FrontendPlugin : Plugin
{
    public override Task Configure(IServiceProvider provider, object? host = null) => Task.CompletedTask;
    public override void Install(IServiceCollection services) { }
}
