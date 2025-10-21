# LowlandTech Plugins

Lightweight, test-driven plugin framework for .NET 9 (C# 13) with first-class ASP.NET Core integration. Build features as independently versioned plugins, discover and load them via configuration, and wire them into your app through a small, well-defined lifecycle.

— Core contracts live in `src/lowlandtech.plugins`
— ASP.NET Core integration in `src/LowlandTech.Plugins.AspNetCore`
— Samples in `samples/`
— Tests in `tests/`

Supported runtime/tooling
- .NET 9 (C# 13)

## Problem

Modern apps need to ship features quickly without turning into tangled monoliths. Teams want to:
- Add or remove capabilities at runtime without invasive code changes.
- Keep dependency registration, async initialization, and host wiring predictable.
- Discover plugins from configuration and load them reliably across environments.
- Validate plugin identity and avoid conflicts between modules.

Doing this consistently with DI, configuration, and host integration is error-prone without a clear contract and lifecycle.

## Solution

LowlandTech Plugins provides:
- Simple contract: `IPlugin` and a base `Plugin` type with a three-phase lifecycle: `Install`, `ConfigureContext`, `Configure`.
- Clean integration: `IServiceCollection.AddPlugins()` and `WebApplication.UsePlugins()` for ASP.NET Core; Lamar support is available in the core package.
- Config-driven discovery: Reads a `Plugins` section from configuration, attempts assembly-by-name or file discovery, and instantiates `IPlugin` types.
- Safety and consistency: Guarded plugin IDs via `[PluginId]` attribute; duplicate IDs are rejected.
- Proven by tests: Comprehensive specs under `tests/` for discovery, validation, lifecycle, error handling, and Lamar usage.

## Example

1) Define a plugin:

```csharp
// samples/lowlandtech.sample.backend/BackendPlugin.cs
using LowlandTech.Plugins;
using LowlandTech.Plugins.Types;

[PluginId("306b92e3-2db6-45fb-99ee-9c63b090f3fc")]
public class BackendPlugin : Plugin
{
    public override void Install(IServiceCollection services)
    {
        services.AddSingleton<BackendActivity>();
    }

    public override Task Configure(IServiceProvider container, object? host = null)
    {
        if (host is WebApplication app)
        {
            app.MapGet("/weatherforecast", () => new[] { "Sunny", "Cloudy" });
        }
        return Task.CompletedTask;
    }
}
```

2) Configure plugins (optional when adding explicitly):

```json
// appsettings.json
{
  "Plugins": [
    { "Name": "LowlandTech.Sample.Backend", "IsActive": true }
  ]
}
```

3) Wire up in ASP.NET Core:

```csharp
// samples/lowlandtech.sample.api/Program.cs
var builder = WebApplication.CreateBuilder(args);

// Register by type (explicit)
builder.Services.AddPlugin<BackendPlugin>();

// Or discover from configuration (appsettings.json -> Plugins)
builder.Services.AddPlugins();

var app = builder.Build();

// Run plugin Configure(...) against the host
app.UsePlugins();

app.Run();
```

Run it:
- `dotnet build`
- `dotnet run --project samples/lowlandtech.sample.api`

## Technical Details

- Projects
  - Core: `src/lowlandtech.plugins` — `IPlugin`, `Plugin`, options, guards, and Lamar-focused helpers (`ServiceRegistry`, `IContainer`).
  - ASP.NET Core: `src/LowlandTech.Plugins.AspNetCore` — `IServiceCollection.AddPlugins()`, `IServiceCollection.AddPlugin<T>()`, and `WebApplication.UsePlugins()`.
- Lifecycle (from `src/lowlandtech.plugins/IPlugin.cs`)
  - `Install(IServiceCollection)` — register services.
  - `ConfigureContext(IServiceCollection)` — async context setup (optional).
  - `Configure(IServiceProvider, object? host)` — finalize wiring; `host` can be `WebApplication`.
- Discovery (`src/LowlandTech.Plugins.AspNetCore/Extensions/PluginExtensions.cs`)
  - Reads `Plugins` from configuration into `PluginConfig` (`Name`, `IsActive`).
  - Attempts assembly resolution by name, then searches candidate paths; creates the first concrete `IPlugin` type found.
  - Assigns `plugin.Name` and `plugin.IsActive`; logs progress; falls back to DLL scan if needed.
- Registration helpers
  - ASP.NET Core: `services.AddPlugin(new MyPlugin())`, `services.AddPlugin<MyPlugin>()`, `services.AddPlugins()`; `app.UsePlugins()` to invoke `Configure(...)`.
  - Lamar (core): `ServiceRegistry.AddPlugin(...)`, `ServiceRegistry.AddPlugins()`, `IContainer.UsePlugins(...)` for scenarios using Lamar.
- Identity and validation
  - `[PluginId("<guid>")]` on the plugin type provides a stable ID (`src/lowlandtech.plugins/Types/PluginId.cs`).
  - `Guard.Against.MissingPluginId(...)` enforces presence and rejects duplicates when adding plugins.
- Configuration shape
  - `Plugins: [ { "Name": "<AssemblyOrNamespace>", "IsActive": true } ]`.
  - Nested `Plugins:Plugins` is tolerated for compatibility in tests.
- Samples
  - API host: `samples/lowlandtech.sample.api` (uses `AddPlugins` + `UsePlugins`).
  - Backend plugin: `samples/lowlandtech.sample.backend` (example plugin with `[PluginId]`).

## Quick Start

1) Build: `dotnet build`
2) Test: `dotnet test` (or filter, e.g., `--filter "VCHIP-0010-UC04"`)
3) Run sample API: `dotnet run --project samples/lowlandtech.sample.api`

## Local build & packaging

This repository includes `build.ps1`, a helper to bump versions, build, and pack projects to a local folder for consumption by other local projects.

Basic usage (pack core projects to C:\Workspaces\Packages):

- Pack both main libraries and write packages to the local folder:
  `.uild.ps1 -ProjectFile all -OutputPath 'C:\Workspaces\Packages'`

- Pack a single project (e.g. AspNetCore integration):
  `.uild.ps1 -ProjectFile 'src\LowlandTech.Plugins.AspNetCore\LowlandTech.Plugins.AspNetCore.csproj'`

- Increment the minor version while packing:
  `.uild.ps1 -VersionIncrement Minor`

Notes
- The script updates `Version` / `PackageVersion` in the specified .csproj files. Commit the changes if you want them recorded in git.
- The default output path is `C:\Workspaces\Packages`; clear that folder before running the script if you want only the freshly-produced packages:
  `Remove-Item 'C:\Workspaces\Packages\*' -Force`
- If you prefer not to produce symbol packages (`.snupkg`), either remove `IncludeSymbols` / `SymbolPackageFormat` from the project file(s) or adjust the pack step to suppress symbols.

This section provides a quick way to publish local NuGet packages for downstream development and CI validation.

## Contributing

- Follow the test-first style in `tests/` and keep changes small.
- Use `@VCHIP-xxxx` annotations in tests for traceability when applicable.

## License

See `LICENSE` for details.

## Contact

Repo: https://github.com/lowlandtech/lowlandtech.plugins

