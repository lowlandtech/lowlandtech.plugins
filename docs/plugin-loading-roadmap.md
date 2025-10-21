# Roadmap: Runtime Plugin Loading (NuGet, Roslyn, Naked DLLs)

This roadmap outlines how to evolve LowlandTech Plugins from project-reference-based development to true runtime plugin loading, enabling add/remove/upgrade without recompiling the host. It covers three loading modes:

- NuGet packages (download/resolve at runtime)
- Roslyn-generated assemblies (compiled on the fly)
- Naked DLLs (local files/folders)

The plan preserves existing behavior and extends configuration to support multiple sources.

## Problem Statement

The current implementation assumes plugins are referenced as projects/assemblies at build time. We need to:
- Add or remove plugins at runtime without rebuilding the host.
- Resolve plugin dependencies reliably in isolation.
- Support multiple sources: NuGet, local DLLs, and dynamic Roslyn compilation.
- Maintain a consistent plugin lifecycle and identity validation.

## Goals

- Unified configuration and management for all plugin sources.
- Isolated, unloadable plugin load contexts (per plugin) to enable updates/removal.
- Minimal host coupling: no compile-time dependency on plugin projects.
- Backward compatibility with the current `Plugins` configuration where possible.

## Non‑Goals (initially)

- Full process sandboxing or OS‑level isolation (can be a future enhancement).
- Signed code enforcement for all sources (will add verification hooks; strict policy optional).

---

## Proposed Configuration Schema

Extend `Plugins` items to include a `Source` discriminator. Backward‑compatible default is `AssemblyName` (current behavior).

```json
{
  "Plugins": [
    { "Source": "AssemblyName", "Name": "LowlandTech.Sample.Backend", "IsActive": true },
    { "Source": "File", "Path": "plugins/Contoso.Backoffice.dll", "IsActive": true },
    { "Source": "Directory", "Path": "plugins/Acme.Reports/", "IsActive": true },
    { "Source": "NuGet", "PackageId": "Contoso.Backoffice", "Version": "1.2.3", "IsActive": true },
    {
      "Source": "Roslyn",
      "Language": "CSharp",
      "CodePath": "plugins/dynamic/MyPlugin.cs",
      "References": [ "LowlandTech.Plugins.dll" ],
      "IsActive": true
    }
  ]
}
```

Optional manifest (for `File`/`Directory`/`NuGet`) to reduce scanning:

```json
// plugins/Contoso.Backoffice/plugin.json
{
  "Id": "6a8b3c7b-28f5-4954-a4b4-1f8a2a2cf234",
  "Assembly": "Contoso.Backoffice.dll",
  "EntryType": "Contoso.Backoffice.BackofficePlugin",
  "Name": "Contoso.Backoffice",
  "Version": "1.2.3"
}
```

---

## Architecture Overview

- PluginManager (new): Orchestrates discovery, load, activation, unload, and updates.
- PluginStore: Local folder for installed packages/compiled outputs (e.g., `%LOCALAPPDATA%/LowlandTech/Plugins` or `./plugins`).
- PluginLoadContext (per plugin): Collectible `AssemblyLoadContext` for isolation and unload.
- Assembly resolution:
  - `AssemblyDependencyResolver` for `File`/`Directory`/`NuGet` installs.
  - Explicit reference map for Roslyn-generated assemblies.
- Integration points:
  - `IServiceCollection.AddPlugins()` reads extended config and defers to PluginManager.
  - `WebApplication.UsePlugins()` triggers `Configure(...)` on loaded plugins.

---

## Phased Plan

### Phase 1: Naked DLL/Directory Loader

- Add support for `Source=File` and `Source=Directory`.
- For each plugin entry:
  - Create a `PluginLoadContext` rooted at the folder containing the DLL.
  - Use `AssemblyDependencyResolver` to resolve dependencies within that folder first.
  - Load the main assembly; locate the first concrete `IPlugin` type (or use manifest `EntryType`).
  - Enforce `[PluginId]` presence via existing Guard; register plugin; keep handle to the load context to enable unload.
- Add tests for: load, double-load prevention, unload and reload, resolution of private dependencies.

### Phase 2: NuGet Loader

- Use NuGet.Client APIs to restore and install a package to `PluginStore`.
  - `NuGet.Protocol` for repository access, `NuGet.Packaging` for extraction.
  - Pick the nearest TFM for the host (`net9.0`), respect RIDs if present.
  - Extract to `PluginStore/<package-id>/<version>/` and record a manifest.
- Load via a `PluginLoadContext` rooted at the extracted folder.
- Support upgrade/downgrade by unloading the previous version’s context and replacing it on disk.
- Add tests for: install, update, rollback on failure, dependencies resolved from the package’s lib/ folder.

### Phase 3: Roslyn Loader

- Compile provided source (`Code`, `CodePath`, or a folder) using Roslyn (`Microsoft.CodeAnalysis.CSharp`).
- Reference the host’s BCL via `TRUSTED_PLATFORM_ASSEMBLIES` and `LowlandTech.Plugins.dll` (from `typeof(IPlugin).Assembly.Location`).
- Emit to `PluginStore/dynamic/<name>/<hash>/` and load via `PluginLoadContext`.
- Add tests for: compile success/failure, diagnostics surfaced, caching of identical inputs, unload/recompile cycle.

### Phase 4: Management API/CLI

- API: `POST /plugins/install`, `DELETE /plugins/{id}`, `POST /plugins/upgrade`, `GET /plugins`.
- CLI (optional): `plugins install --nuget Contoso.Backoffice --version 1.2.3`, `plugins add-file ./plugins/X.dll`, `plugins unload <id>`.
- File watcher for `plugins/` directory to auto-load/unload on changes.

### Phase 5: Security & Trust

- NuGet signature verification (configurable trust policy).
- Optional plugin signing/verification (file/dir installs).
- Source allowlist/denylist (package IDs, publishers, paths).
- Option to require manifest + `[PluginId]` with stable GUID.
- Future: process isolation (out-of-proc host) for untrusted plugins.

### Phase 6: Observability & Test Coverage

- Structured logging during discovery and resolution; metrics for load times and failures.
- Spec coverage (VCHIP) for configuration variants, resolution paths, errors, and unload flows.

---

## Detailed Design Notes

### PluginLoadContext (collectible)

```csharp
public sealed class PluginLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;

    public string BasePath { get; }

    public PluginLoadContext(string basePath) : base(isCollectible: true)
    {
        BasePath = basePath;
        _resolver = new AssemblyDependencyResolver(basePath);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var path = _resolver.ResolveAssemblyToPath(assemblyName);
        if (path is not null)
        {
            return LoadFromAssemblyPath(path);
        }
        return null;
    }
}
```

Guidance:
- One `PluginLoadContext` per plugin instance/version.
- Never load host/shared assemblies (e.g., LowlandTech.Plugins) from plugin folder; rely on default context to avoid type identity conflicts.
- Keep references scoped so contexts can be collected on unload.

### Naked DLL/Directory Loader

Steps:
- Determine plugin root (file’s directory or provided folder).
- Instantiate `PluginLoadContext(root)` and `LoadFromAssemblyPath(mainDll)`.
- If `plugin.json` exists, use `EntryType`; else scan for the first non-abstract `IPlugin` type.
- Validate `[PluginId]`; call existing `services.AddPlugin(plugin)`.
- Track `plugin.Id -> loadContext` in `PluginManager` for unload.

### NuGet Loader (runtime restore + isolate)

High-level flow:
1. Resolve package from configured sources (default nuget.org; allow custom feed URLs).
2. Download and extract to `PluginStore/<id>/<version>/`.
3. Determine TFM/RID assets (nearest target selection).
4. Create `PluginLoadContext` rooted at the extracted lib folder; load the primary assembly.
5. Discover `IPlugin` type and register.

Key APIs (NuGet.Client):
- `Repository.Factory.GetCoreV3(source)` / `PackageMetadataResource` / `DownloadResource`.
- `PackageExtraction.ExtractPackageAsync(...)`.
- `FrameworkReducer` to choose nearest TFM for `net9.0`.

Notes:
- Cache installed packages; skip download if already present.
- Implement retries and checksum verification.
- Provide a manifest for quick rehydrate and to pin entry assembly/type.

### Roslyn Loader (compile on the fly)

High-level flow:
1. Collect sources (single file or folder).
2. Build reference set:
   - Split `AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")` for BCL refs.
   - Add `typeof(IPlugin).Assembly.Location` and any additional configured references.
3. Compile with `CSharpCompilation` to an assembly file under `PluginStore/dynamic/<name>/<hash>/`.
4. Load via `PluginLoadContext` and register `IPlugin`.

Security:
- Treat as privileged by default; gate behind config and role checks.
- Optionally restrict available references to prevent escalation.

### Unloading & Updates

- Keep `PluginHandle` with: `Id`, `Version`, `LoadContext`, `Assemblies`, `DisposableResources`.
- Unload flow: stop plugin activity if applicable, detach from host, dispose resources, `LoadContext.Unload()`, then `GC.Collect()` and `GC.WaitForPendingFinalizers()`.
- Update flow: install new version side‑by‑side, swap registrations, then unload old context.

---

## Integration with Existing Extensions

- `IServiceCollection.AddPlugins()`
  - Parse extended config; for each entry call the appropriate loader (AssemblyName/File/Directory/NuGet/Roslyn).
  - Preserve backward compatibility for entries that only provide `Name` + `IsActive`.

- `WebApplication.UsePlugins()`
  - Unchanged: enumerate registered `IPlugin` and call `Configure(...)`.

- Lamar support
  - Mirror new loaders in the Lamar path (`ServiceRegistry.AddPlugins()`), or reuse the common `PluginManager` behind both.

---

## Test Strategy (VCHIP‑style examples)

- UC: File Loader
  - SC: Loads plugin from DLL with private dependency in sibling folder.
  - SC: Rejects plugin without `[PluginId]`.
  - SC: Unloads and reloads updated DLL.

- UC: NuGet Loader
  - SC: Installs package, resolves dependencies, loads plugin.
  - SC: Upgrades to newer version; old context unloaded.
  - SC: Fails gracefully on bad TFM or missing assets.

- UC: Roslyn Loader
  - SC: Compiles simple plugin; loads and runs.
  - SC: Emits diagnostics for compile errors; no partial load.
  - SC: Recompiles after code change; unloads prior context.

---

## Risks & Mitigations

- Type identity issues when multiple copies of the same assembly load across contexts →
  - Load shared contracts (e.g., LowlandTech.Plugins) only in default context; avoid shadow copies in plugin folders.

- Memory retention preventing unload →
  - Avoid static caches in plugins; provide guidance and tests that assert collectible unload.

- NuGet supply-chain risks →
  - Add signature verification and allowlist policy; optional offline/locked mode.

- Large plugin dependency graphs →
  - Use `AssemblyDependencyResolver` and `deps.json` when available; log resolution steps.

---

## Feature Management

Enable turning features on/off per environment, permissions, audience, and configuration. This applies to both whole plugins and granular capabilities (routes, services, activities) within a plugin.

### Goals
- Centralized, declarative toggles for plugins and features.
- Environment-, audience-, and permission-aware enablement.
- Live toggle support where safe; otherwise trigger plugin reload.

### Building Blocks
- Use `Microsoft.FeatureManagement` and `Microsoft.FeatureManagement.AspNetCore`.
  - Register with `services.AddFeatureManagement()`.
  - Access flags via `IFeatureManager` / `IFeatureManagerSnapshot`.
  - Built-in filters: `TimeWindow`, `Percentage`, `Targeting`.
- Add custom filters:
  - `EnvironmentFeatureFilter` (checks `IHostEnvironment.EnvironmentName`).
  - `PermissionFeatureFilter` (checks claims/roles via `IAuthorizationService`).
  - `ConfigFeatureFilter` (checks arbitrary config flags).

### Configuration

```json
{
  "FeatureManagement": {
    // Plugin-level gate
    "Plugins.LowlandTech.Sample.Backend": true,

    // Feature-level gate inside a plugin
    "Features.Backend.Weather": {
      "EnabledFor": [
        { "Name": "Environment", "Parameters": { "Allowed": ["Development", "Staging"] } },
        { "Name": "Targeting", "Parameters": {
          "Audience": {
            "Groups": ["beta", "staff"],
            "DefaultRolloutPercentage": 25
          }
        }}
      ]
    }
  }
}
```

Example custom filter (Environment):

```csharp
public sealed class EnvironmentFeatureFilter : IFeatureFilter
{
    private readonly IHostEnvironment _env;
    public EnvironmentFeatureFilter(IHostEnvironment env) => _env = env;

    private sealed class Params { public string[]? Allowed { get; set; } }
    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
    {
        var p = context.Parameters.Get<Params>();
        return Task.FromResult(p?.Allowed?.Contains(_env.EnvironmentName, StringComparer.OrdinalIgnoreCase) == true);
    }
}
```

### Integration Points

- Plugin activation
  - During discovery, compute activation as `config.IsActive && featureManager.IsEnabledAsync($"Plugins.{config.Name}")`.
  - When a toggle flips off, schedule plugin unload; when on, attempt load.

- Service registration (Install)
  - Guard registrations with feature flags (soft gating) or split registrations into feature-scoped modules.
  - For live toggles, prefer gating at resolution/use sites (e.g., decorators) rather than adding/removing service descriptors.

- Context/host wiring (Configure/ConfigureContext)
  - Minimal APIs: check `IFeatureManager` before mapping endpoints, or wrap handlers.
  - MVC: use `[FeatureGate("Features.Backend.Weather")]` attributes.

Minimal API example:

```csharp
public override async Task Configure(IServiceProvider sp, object? host = null)
{
    if (host is WebApplication app)
    {
        var features = sp.GetRequiredService<IFeatureManagerSnapshot>();
        if (await features.IsEnabledAsync("Features.Backend.Weather"))
        {
            app.MapGet("/weatherforecast", () => /* ... */);
        }
    }
}
```

### Live Toggles and Reload Strategy
- Prefer soft gates (feature checks at runtime) for handlers/behaviors that are safe to disable without container rebuild.
- For structural changes (service graph, large route sets), expose a `PluginManager.Reload(pluginId)` to rebuild the affected plugin context.
- Use `IFeatureManagerSnapshot` for request-scoped checks; use `IFeatureManager`/watchers to react to global changes.

### Permissions and Audience
- Permissions: implement `PermissionFeatureFilter` that evaluates claims/roles/policies via `IAuthorizationService`.
- Audience: use built-in `Targeting` filter; map groups to identity provider roles/claims.
- Combine filters via `EnabledFor` arrays to model complex rollout policies.

---

## Acceptance Criteria

- Load plugins from File/Directory, NuGet, and Roslyn sources via configuration without host rebuild.
- Enable unload and upgrade with no host restart (within supported boundaries).
- Maintain existing lifecycle (`Install`, `ConfigureContext`, `Configure`) and ID validation.
- Comprehensive logging and tests for discovery, resolution, errors, and unload.

---

## Open Questions

- Default PluginStore location and cleanup policy?
- Do we require signed packages/files by default in production?
- Should Roslyn be available in production or restricted to development/test?
