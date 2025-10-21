# UC00 - Plugin Configuration Tests

## Overview
This test suite covers the Plugin Configuration functionality (UC00) for the LowlandTech Plugin Framework. Tests verify that plugin configurations can be properly loaded, parsed, and bound from appsettings.json.

## Test Scenarios

### SC01 - Load Plugin Configuration
**Spec ID**: VCHIP-0010-UC00-SC01
**Purpose**: Verify that plugin configuration can be loaded from appsettings.json
**UAC Coverage**: UAC001-UAC002

**Tests**:
- UAC001: PluginOptions should be populated
- UAC002: PluginOptions.Plugins list should contain all configured plugins

---

### SC02 - Parse Plugin Name
**Spec ID**: VCHIP-0010-UC00-SC02
**Purpose**: Verify plugin names are correctly extracted from configuration
**UAC Coverage**: UAC001-UAC002

**Tests**:
- UAC001: Plugin name should match configuration value
- UAC002: Name should not include .dll extension

---

### SC03 - Parse IsActive Flag
**Spec ID**: VCHIP-0010-UC00-SC03
**Purpose**: Verify IsActive flag controls plugin loading
**UAC Coverage**: UAC001-UAC002

**Tests**:
- UAC001: IsActive flag should be correctly parsed
- UAC002: Inactive plugins should not be loaded

---

### SC04 - Use PluginOptions Constant
**Spec ID**: VCHIP-0010-UC00-SC04
**Purpose**: Verify PluginOptions.Name constant usage
**UAC Coverage**: UAC001-UAC002

**Tests**:
- UAC001: Constant should equal "Plugins"
- UAC002: Configuration should bind correctly using constant

---

### SC05 - Handle Multiple Plugin Configurations
**Spec ID**: VCHIP-0010-UC00-SC05
**Purpose**: Verify multiple plugins can be configured
**UAC Coverage**: UAC001-UAC003

**Tests**:
- UAC001: All PluginConfig objects should be created
- UAC002: Correct count of active plugins
- UAC003: Correct count of inactive plugins

---

### SC06 - Empty Plugins Array
**Spec ID**: VCHIP-0010-UC00-SC06
**Purpose**: Verify graceful handling of empty configuration
**UAC Coverage**: UAC001-UAC003

**Tests**:
- UAC001: PluginOptions.Plugins should be empty
- UAC002: No plugins should be loaded
- UAC003: No errors should occur

---

### SC07 - PluginOptions Initialization
**Spec ID**: VCHIP-0010-UC00-SC07
**Purpose**: Verify default PluginOptions initialization
**UAC Coverage**: UAC001-UAC002

**Tests**:
- UAC001: Plugins list should be initialized as empty list
- UAC002: List should not be null

---

### SC08 - Configuration Binding
**Spec ID**: VCHIP-0010-UC00-SC08
**Purpose**: Verify IConfiguration binding to PluginOptions
**UAC Coverage**: UAC001-UAC002

**Tests**:
- UAC001: PluginOptions should be populated correctly
- UAC002: All plugin configurations should be deserialized properly

---

## Running the Tests

### Prerequisites
- .NET 9.0 SDK or later
- xUnit test runner
- Shouldly assertion library

### Run All UC00 Tests
```bash
dotnet test --filter "VCHIP-0010-UC00"
```

### Run Specific Scenario
```bash
dotnet test --filter "VCHIP-0010-UC00-SC01"
```

## Test Structure

Each test follows the **WhenTestingFor** pattern:

```csharp
[Scenario(
    specId: "VCHIP-0010-UC00-SC##",
    title: "Test title",
    given: "Given condition",
    when: "When action",
    then: "Then expected result")]
public sealed class SC##_TestName : WhenTestingFor<ConfigurationTestFixture>
{
    protected override ConfigurationTestFixture For() => new ConfigurationTestFixture();
    protected override void Given() { /* Setup */ }
    protected override void When() { /* Action */ }

    [Fact]
    [Then("Assertion description", "UAC###")]
    public void Test_Method() { /* Assertion */ }
}
```

## Dependencies

- **Microsoft.Extensions.Configuration** - Configuration abstraction
- **Microsoft.Extensions.Configuration.Binder** - Configuration binding
- **xUnit** - Test framework
- **Shouldly** - Assertion library
- **LowlandTech.Plugins** - Plugin framework under test

## Coverage

| Feature Area | Scenarios | UAC Count | Status |
|-------------|-----------|-----------|--------|
| Configuration Loading | SC01 | 2 | ✅ Complete |
| Name Parsing | SC02 | 2 | ✅ Complete |
| IsActive Flag | SC03 | 2 | ✅ Complete |
| Constants | SC04 | 2 | ✅ Complete |
| Multiple Plugins | SC05 | 3 | ✅ Complete |
| Edge Cases | SC06-SC07 | 5 | ✅ Complete |
| Binding | SC08 | 2 | ✅ Complete |
| **Total** | **8** | **18** | **✅ Complete** |

## Future Enhancements

Additional scenarios to implement:
- SC09: Environment-specific configuration (Development)
- SC10: Environment-specific configuration (Production)
- SC11: Validation for missing Name
- SC12: Configuration with extra properties
- SC13: Plugin path resolution
- SC14: Case sensitivity in configuration section names

## Notes

- Tests use in-memory configuration for isolation
- No external dependencies required for these tests
- Tests are fast and can run in parallel
- All tests follow the Given-When-Then pattern with UAC tracking
