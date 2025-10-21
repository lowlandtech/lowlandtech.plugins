# UC04 - Plugin Lifecycle Management Tests

## Overview
This test suite covers the Plugin Lifecycle Management functionality (UC04) for the LowlandTech Plugin Framework. Tests verify that plugins follow a well-defined lifecycle with proper execution of Install, ConfigureContext, and Configure phases.

## Test Scenarios

### SC01 - Execute Install Phase
**Spec ID**: VCHIP-0010-UC04-SC01  
**Purpose**: Verify Install method execution during plugin registration  
**UAC Coverage**: UAC001-UAC003

**Tests**:
- UAC001: Plugin should register its services in the service collection
- UAC002: Plugin services should be available for dependency injection
- UAC003: Install method should complete successfully

**Implementation**: `SC01_InstallPhase.cs`

---

### SC02 - Execute ConfigureContext Phase
**Spec ID**: VCHIP-0010-UC04-SC02  
**Purpose**: Verify asynchronous ConfigureContext method execution  
**UAC Coverage**: UAC004-UAC006

**Tests**:
- UAC004: Plugin should perform asynchronous context configuration
- UAC005: ConfigureContext method should complete without errors
- UAC006: Method should return a completed Task

**Implementation**: `SC02_ConfigureContextPhase.cs`

---

### SC03 - Execute Configure Phase
**Spec ID**: VCHIP-0010-UC04-SC03  
**Purpose**: Verify Configure method with service provider  
**UAC Coverage**: UAC007-UAC009

**Tests**:
- UAC007: Plugin should have access to all registered services
- UAC008: Plugin should complete its configuration
- UAC009: Configure method should return a completed Task

**Implementation**: `SC03_ConfigurePhase.cs`

---

### SC04 - Configure with Host (ASP.NET Core)
**Spec ID**: VCHIP-0010-UC04-SC04  
**Purpose**: Verify Configure with WebApplication host object  
**UAC Coverage**: UAC010-UAC013

**Tests**:
- UAC010: Plugin should have access to the WebApplication instance
- UAC011: Plugin should be able to configure routes
- UAC012: Plugin should be able to configure middleware
- UAC013: Configure method should complete successfully

**Implementation**: `SC04_ConfigureWithHost.cs`

---

### SC05 - Complete Lifecycle Flow
**Spec ID**: VCHIP-0010-UC04-SC05  
**Purpose**: Verify complete lifecycle execution order  
**UAC Coverage**: UAC014-UAC017

**Tests**:
- UAC014: Install phase should execute first
- UAC015: ConfigureContext phase should execute after Install
- UAC016: Configure phase should execute last
- UAC017: All phases should complete successfully

**Implementation**: `SC05_CompleteLifecycleFlow.cs`

---

### SC06 - Register Services During Install
**Spec ID**: VCHIP-0010-UC04-SC06  
**Purpose**: Verify plugin service registration with different lifetimes  
**UAC Coverage**: UAC018-UAC021

**Tests**:
- UAC018: Plugin should add singleton services
- UAC019: Plugin should add scoped services
- UAC020: Plugin should add transient services
- UAC021: All registered services should be resolvable

**Implementation**: `SC06_RegisterServices.cs`

---

## Plugin Lifecycle Phases

### 1. Install Phase
- **When**: During plugin registration via `AddPlugin()` or `AddPlugins()`
- **Purpose**: Register services in the DI container
- **Method**: `void Install(IServiceCollection services)`
- **Synchronous**: Must complete before service provider is built

### 2. ConfigureContext Phase
- **When**: After Install, before Configure (optional)
- **Purpose**: Perform async initialization and context setup
- **Method**: `Task ConfigureContext(IServiceCollection services)`
- **Asynchronous**: Can perform async operations

### 3. Configure Phase
- **When**: After service provider is built
- **Purpose**: Configure the application using resolved services
- **Method**: `Task Configure(IServiceProvider container, object? host)`
- **Asynchronous**: Can access all registered services
- **Host Parameter**: WebApplication for ASP.NET Core apps, null for others

---

## Test Infrastructure

### LifecycleTestFixture
Base test fixture for lifecycle tests.

### TestLifecyclePlugin
Test plugin that tracks all lifecycle method calls and provides:
- Flags to verify each phase was called
- Exception tracking for error scenarios
- Access to received parameters (ServiceProvider, Host)
- Route and middleware configuration tracking
- Execution order tracking via callback

### ServiceRegistrationPlugin
Plugin that demonstrates service registration with different lifetimes:
- Singleton services (shared across application)
- Scoped services (per request/scope)
- Transient services (new instance each time)

### Test Services
- `TestPluginService` - Service registered during Install
- `SingletonTestService` - Singleton lifetime demonstration
- `ScopedTestService` - Scoped lifetime demonstration
- `TransientTestService` - Transient lifetime demonstration

---

## Running the Tests

### Prerequisites
- .NET 9.0 SDK or later
- xUnit test runner
- Shouldly assertion library

### Run All UC04 Tests
```bash
dotnet test --filter "VCHIP-0010-UC04"
```

### Run Specific Scenario
```bash
dotnet test --filter "VCHIP-0010-UC04-SC01"
```

### Run All Lifecycle Tests
```bash
dotnet test --filter "Lifecycle"
```

---

## Test Patterns

Each test follows the **WhenTestingForV2** pattern:

```csharp
[Scenario(
    specId: "VCHIP-0010-UC04-SC##",
    title: "Test title",
    given: "Given condition",
    when: "When action",
    then: "Then expected result")]
public sealed class SC##_TestName : WhenTestingForV2<LifecycleTestFixture>
{
    protected override LifecycleTestFixture For() => new();
    protected override void Given() { /* Setup */ }
    protected override void When() { /* Action */ }

    [Fact]
    [Then("Assertion description", "UAC###")]
    public void Test_Method() { /* Assertion */ }
}
```

---

## Dependencies

- **Microsoft.Extensions.DependencyInjection** - Service registration and DI
- **Microsoft.AspNetCore.App** - ASP.NET Core integration (SC04)
- **xUnit** - Test framework
- **Shouldly** - Assertion library
- **LowlandTech.Plugins** - Plugin framework under test
- **LowlandTech.Plugins.AspNetCore** - ASP.NET Core extensions

---

## Coverage Summary

| Feature Area | Scenarios | UAC Count | Status |
|-------------|-----------|-----------|--------|
| Install Phase | SC01 | 3 | ? Complete |
| ConfigureContext | SC02 | 3 | ? Complete |
| Configure Phase | SC03 | 3 | ? Complete |
| ASP.NET Core Host | SC04 | 4 | ? Complete |
| Lifecycle Order | SC05 | 4 | ? Complete |
| Service Registration | SC06 | 4 | ? Complete |
| **Total** | **6** | **21** | **? Complete** |

---

## Future Scenarios (SC07-SC16)

Remaining scenarios to implement:
- SC07: Virtual ConfigureContext default implementation
- SC08: UsePlugins executes Configure on all plugins
- SC09: Plugin Configure with null host
- SC10: Plugin accesses dependencies during Configure
- SC11: Handle exception during Install phase
- SC12: Handle exception during ConfigureContext phase
- SC13: Handle exception during Configure phase
- SC14: Plugin metadata preservation
- SC15: Multiple plugins execute independently
- SC16: Plugin Assemblies collection accessibility

---

## Best Practices

1. **Install Phase**
   - Only register services, don't resolve them
   - Keep synchronous and fast
   - Don't throw exceptions unless critical

2. **ConfigureContext Phase**
   - Use for async initialization
   - Can perform I/O operations
   - Optional - base implementation returns completed Task

3. **Configure Phase**
   - Access resolved services via IServiceProvider
   - Configure routes, middleware (ASP.NET Core)
   - Handle null host gracefully for non-web scenarios

4. **Service Lifetimes**
   - Singleton: Shared across application lifetime
   - Scoped: Per HTTP request or explicit scope
   - Transient: New instance every resolution

---

## Notes

- Tests use both `LifecycleTestFixture` and `AspNetCoreTestFixture`
- SC04 requires WebApplication builder for host testing
- All async tests properly await Task completion
- Service lifetime tests verify reference equality
- Execution order tracking validates lifecycle sequence

---

## License

© 2025 **LowlandTech Foundry**  
Part of the Vylyrian ecosystem.  
Licensed under the LowlandTech Foundry Developer License (LT-FDL).
