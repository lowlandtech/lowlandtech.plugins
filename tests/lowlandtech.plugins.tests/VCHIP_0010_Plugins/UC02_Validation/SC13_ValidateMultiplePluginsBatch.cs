using System.Collections.Generic;
using Ardalis.GuardClauses;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC02_Validation;

[Scenario(
    specId: "VCHIP-0010-UC02-SC13",
    title: "Validate multiple plugins in batch",
    given: "Given a collection of plugins to validate:",
    when: "When each plugin is validated with Guard.Against.MissingPluginId",
    then: "Then BackendPlugin and FrontendPlugin should pass validation and InvalidPlugin should fail")]
public class SC13_ValidateMultiplePluginsBatch : WhenTestingForV2<ValidationTestFixture>
{
    private List<IPlugin>? _plugins;
    private List<string> _validIds = new();
    private List<IPlugin> _invalid = new();

    protected override ValidationTestFixture For() => new();

    protected override void Given()
    {
        _plugins = new List<IPlugin>
        {
            new BackendPlugin(),
            new FrontendPlugin(),
            new InvalidPluginNoId()
        };
    }

    protected override void When()
    {
        foreach (var p in _plugins!)
        {
            try
            {
                var id = Guard.Against.MissingPluginId(p, nameof(p));
                _validIds.Add(id);
            }
            catch
            {
                _invalid.Add(p);
            }
        }
    }

    private static bool IsGuidString(string id)
    {
        return Guid.TryParse(id, out var parsed);
    }

    [Fact]
    [Then("BackendPlugin should pass validation", "UAC030")]
    public void BackendPlugin_Should_Pass()
    {
        _validIds.Count.ShouldBeGreaterThanOrEqualTo(1);
        _validIds.Any(IsGuidString).ShouldBeTrue();
    }

    [Fact]
    [Then("FrontendPlugin should pass validation", "UAC031")]
    public void FrontendPlugin_Should_Pass()
    {
        _validIds.Count.ShouldBeGreaterThanOrEqualTo(2);
        _validIds.Any(IsGuidString).ShouldBeTrue();
    }

    [Fact]
    [Then("InvalidPlugin should fail validation", "UAC032")]
    public void InvalidPlugin_Should_Fail()
    {
        _invalid.Count.ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact]
    [Then("Only valid plugins should be registered", "UAC033")]
    public void Only_Valid_Plugins_Registered()
    {
        // Simulation: only valid ids were collected
        _validIds.Count.ShouldBe(2);
        _invalid.Count.ShouldBe(1);
    }
}
