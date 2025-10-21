# UC01 - ASP.NET Core Integration Tests

## Overview
This test suite covers the ASP.NET Core integration functionality (UC01) for the LowlandTech Plugin Framework. Tests verify that plugins can be properly registered, configured, and integrated with ASP.NET Core applications.

## Test Scenarios

### SC01 - Add Plugins to Service Collection
**Spec ID**: VCHIP-0010-UC01-SC01
**Purpose**: Verify plugins can be added to ASP.NET Core service collection
**UAC Coverage**: UAC001-UAC003

**Tests**:
- UAC001: Plugins should be loaded from configuration
- UAC002: Plugins should be registered in service collection
- UAC003: Plugins should be available through dependency injection

---

### SC02 - Add Plugin by Type
**Spec ID**: VCHIP-0010-UC01-SC02
**Purpose**: Verify plugins can be added by type using AddPlugin<T>()
**UAC Coverage**: UAC004-UAC006

**Tests**:
- UAC004: Plugin instance should be created
- UAC005: Plugin should be registered in service collection
- UAC006: Plugin Install method should be called

---

### SC03 - Add Plugin Instance
**Spec ID**: VCHIP-0010-UC01-SC03
**Purpose**: Verify plugin instances can be added directly
**UAC Coverage**: UAC007-UAC009

**Tests**:
- UAC007: Plugin instance should be registered directly
- UAC008: Plugin Install method should be called
- UAC009: Plugin should be available for dependency injection

---

### SC04 - Use Plugins with WebApplication
**Spec ID**: VCHIP-0010-UC01-SC04
**Purpose**: Verify UsePlugins() configures all registered plugins
**UAC Coverage**: UAC010-UAC012

**Tests**:
- UAC010: Configure method should be called on each plugin
- UAC011: Each plugin should receive the service provider
- UAC012: Each plugin should receive the WebApplication as host

---

### SC05 - Plugin Registers Routes
**Spec ID**: VCHIP-0010-UC01-SC05
**Purpose**: Verify plugins can register routes during Configure phase
**UAC Coverage**: UAC013-UAC015

**Tests**:
- UAC013: Plugin should register routes using app.MapGet
- UAC014: Routes should be accessible in the application
- UAC015: Requests to plugin routes should be handled correctly

---

### SC06 - Plugin Accesses Services
**Spec ID**: VCHIP-0010-UC01-SC06
**Purpose**: Verify plugins can access ASP.NET Core services
**UAC Coverage**: UAC022-UAC024

**Tests**:
- UAC022: Plugin should be able to resolve ILogger
- UAC023: Plugin should be able to resolve IConfiguration
- UAC024: Plugin should be able to use services for configuration

---

### SC07 - Plugin Null Host Handling
**Spec ID**: VCHIP-0010-UC01-SC07
**Purpose**: Verify plugins handle null host gracefully
**UAC Coverage**: UAC053-UAC055

**Tests**:
- UAC053: Plugin should detect the null host
- UAC054: Plugin should skip WebApplication-specific configuration
- UAC055: Plugin should return Task.CompletedTask

---

## Running the Tests

### Prerequisites
- .NET 9.0 SDK or later
- xUnit test runner
- Shouldly assertion library
- Microsoft.AspNetCore.Mvc.Testing

### Run All UC01 Tests
```bash
dotnet test --filter "FullyQualifiedName~UC01_AspNetCore"
```

### Run Specific Scenario
```bash
dotnet test --filter "FullyQualifiedName~SC01_AddPluginsToServiceCollection"
```

### Run by Namespace
```bash
dotnet test --filter "FullyQualifiedName~LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC01_AspNetCore"
```

## Test Structure

Each test follows the **WhenTestingForV2** or **WhenTestingForAsyncV2** pattern:

```csharp
[Scenario(
    specId: "VCHIP-0010-UC01-SC##",
    title: "Test title",
    given: "Given condition",
    when: "When action",
    then: "Then expected result")]
public sealed class SC##_TestName : WhenTestingForV2<AspNetCoreTestFixture>
{
    protected override AspNetCoreTestFixture For() => new AspNetCoreTestFixture();
    protected override void Given() { /* Setup */ }
    protected override void When() { /* Action */ }

    [Fact]
    [Then("Assertion description", "UAC###")]
    public void Test_Method() { /* Assertion */ }
}
```

## Test Helpers

### TestPlugin
A test implementation of the Plugin base class used for testing:
- Tracks whether Install() and Configure() were called
- Captures service provider and host during Configure
- Registers a test service and test endpoint

### AspNetCoreTestFixture
Provides common setup methods:
- `CreateBuilder()` - Creates WebApplicationBuilder with test configuration
- `CreateAppWithPlugin()` - Creates WebApplication with plugin registered
- `CreateConfiguration()` - Creates IConfiguration with test data

### TestService
A simple service registered by TestPlugin to verify service registration works

## Dependencies

- **Microsoft.AspNetCore.App** - ASP.NET Core framework
- **Microsoft.AspNetCore.Mvc.Testing** - WebApplicationFactory for testing
- **xUnit** - Test framework
- **Shouldly** - Assertion library
- **LowlandTech.Plugins.AspNetCore** - Plugin framework under test

## Coverage

### Implemented Tests

| Feature Area | Scenarios | UAC Count | Status |
|-------------|-----------|-----------|--------|
| Service Collection Registration | SC01-SC03 | 9 | ✅ Complete |
| Plugin Configuration | SC04 | 3 | ✅ Complete |
| Route Registration | SC05 | 3 | ✅ Complete |
| Service Access | SC06 | 3 | ✅ Complete |
| Edge Cases | SC07 | 3 | ✅ Complete |
| **Implemented** | **7/27** | **21/76** | **Core Tests Complete** |

### Remaining Scenarios (Feature File)

| Scenario | Description | UAC | Priority |
|----------|-------------|-----|----------|
| SC08 | Plugin with dependency injection in Configure | UAC025-027 | High |
| SC09 | UsePlugins creates scope for service resolution | UAC028-030 | High |
| SC10 | Plugin integrates with OpenAPI | UAC031-033 | Medium |
| SC11 | Plugin registers endpoint with route attributes | UAC034-036 | Medium |
| SC12 | Sample BackendPlugin weatherforecast endpoint | UAC037-040 | Medium |
| SC13 | Plugin configuration in Development environment | UAC041-042 | Low |
| SC14 | Plugin configuration in Production environment | UAC043-044 | Low |
| SC15 | Plugin adds HTTPS redirection | UAC045-046 | Medium |
| SC16 | Plugin registers minimal API endpoints with parameters | UAC047-049 | Medium |
| SC17 | Plugin registers POST endpoint with request body | UAC050-052 | Medium |
| SC18 | Plugin lifetime in ASP.NET Core | UAC061-063 | High |
| SC19 | Multiple calls to UsePlugins | UAC064-065 | Medium |
| SC20-27 | Additional edge cases and integration tests | UAC056-076 | Low-Medium |

## Future Enhancements

Additional scenarios to implement:
- SC08: Multiple plugins register routes without conflicts
- SC09: Plugin configures middleware during Configure phase
- SC10: Plugin with dependency injection in Configure
- SC11: Plugin integrates with OpenAPI
- SC12: Plugin registers endpoint with route parameters
- SC13: Plugin registers POST endpoint with request body
- SC14: Environment-specific plugin configuration
- SC15: Plugin lifetime in ASP.NET Core

## Test Results

All implemented tests are passing:

```
Test run for C:\Workspaces\lowlandtech.plugins\tests\lowlandtech.plugins.tests\bin\Debug\net9.0\LowlandTech.Plugins.Tests.dll
Tests: 21 passed (21 total)
```

### Test Execution Status
- **SC01_AddPluginsToServiceCollection**: ✅ 3/3 tests passing
- **SC02_AddPluginByType**: ✅ 3/3 tests passing
- **SC03_AddPluginInstance**: ✅ 3/3 tests passing
- **SC04_UsePluginsWithWebApplication**: ✅ 3/3 tests passing
- **SC05_PluginRegistersRoutes**: ✅ 3/3 tests passing
- **SC06_PluginAccessesServices**: ✅ 3/3 tests passing
- **SC07_PluginNullHostHandling**: ✅ 3/3 tests passing

## Notes

- Tests use WebApplication.CreateBuilder() for realistic ASP.NET Core integration
- Some tests start the app to verify endpoints are accessible
- Tests properly dispose of WebApplication and HttpClient resources
- Async tests inherit from WhenTestingForAsyncV2 with CancellationToken parameters
- All tests follow Given-When-Then pattern with UAC tracking
- TestPlugin helper class tracks Install() and Configure() method calls for verification
- Use FullyQualifiedName filters for running tests (xUnit doesn't support tag-based filtering)
