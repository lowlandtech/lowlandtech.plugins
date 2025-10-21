UC07 — Lamar Container Integration

Purpose
- Validate integration of the plugin framework with the Lamar IoC container (non-ASP.NET Core scenarios).
- Ensure discovery, registration and lifecycle (Install / Configure) work when using `ServiceRegistry` and `IContainer`.

Scope
- Scenarios cover discovery from configuration, explicit registration, lifecycle execution, container features (scopes, singletons), dependency chains, validation and assembly tracking.

Key guidance for tests and plugin authors
- Use Lamar-native registration inside plugins when targeting Lamar: prefer `services.For<T>().Use<TImpl>()` and lifetime methods such as `.Singleton()` / `.Scoped()`.
- If a plugin must support Lamar-specific lifecycle overloads, implement both overloads on `Plugin`:
  - `Install(ServiceRegistry services)` for Lamar registrations
  - `Install(IServiceCollection services)` for ASP.NET Core
  - `Configure(IContainer container, object? host = null)` for Lamar run-time configuration
  - `Configure(IServiceProvider provider, object? host = null)` for ASP.NET Core
- Tests in this suite use `TestLifecyclePluginLamar` when a plugin must provide Lamar-specific overloads.

How tests invoke the lifecycle
- Registration: tests call `ServiceRegistry.AddPlugin(IPlugin)` or `ServiceRegistry.AddPlugins()` for discovery.
  - `AddPlugin` will invoke the plugin `Install(ServiceRegistry)` overload when the plugin is a `Plugin` type, so implement the Lamar overload where needed.
- Execution: tests call `IContainer.UsePlugins()` which awaits plugin `Configure(IContainer, object?)`.
  - `UsePlugins` will call the Lamar overload if the concrete plugin inherits `Plugin`.

Running the tests
- From repository root run the test project or filter by UC07:
  - `dotnet test tests\lowlandtech.plugins.tests\LowlandTech.Plugins.Tests.csproj --filter "FullyQualifiedName~UC07_Lamar"`

Notes and gotchas
- Do not mix-only MS DI `AddSingleton` style registrations when relying on Lamar `ServiceRegistry`; prefer Lamar `For<T>().Use<TImpl>()` so registrations are available when building an `IContainer`.
- `Plugin` types used in Lamar scenarios should include a `[PluginId("<guid>")]` attribute — the `Guard` enforces plugin ids.
- Tests in this suite perform some compile-time API-surface checks (extension method availability) by invoking extension methods directly. These are intended to catch accidental signature changes.

Files of interest
- `src/lowlandtech.plugins/Extensions/PluginExtensions.cs` — Lamar `ServiceRegistry` / `IContainer` helpers
- `tests/.../Fixtures/TestLifecyclePluginLamar.cs` — Lamar-compatible test plugin used across UC07
- `tests/.../Fixtures/LamarPlugins.cs` — small helper plugins used by UC07 tests

If further clarification or additional examples are required (for example: decorator/interceptor samples), indicate which area to expand and a concise example will be added to this readme.