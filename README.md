# LowlandTech.<Feature>

> ?? *Agentic, Spec-Driven Plugin for the LowlandTech Foundry Platform*

---

## ?? Overview

**LowlandTech.<Feature>** is a modular plugin implementing one or more **FlowFeatures** within the LowlandTech ecosystem.
Each plugin is self-contained — including its backend logic, UI components, Redux events, and BDD specifications — so that both humans and agents can develop, test, and ship features at machine speed.

| Layer            | Description                                                       |
| ---------------- | ----------------------------------------------------------------- |
| **Abstractions** | Contracts, Actions, Events, DTOs, and public tool manifests.      |
| **Domain**       | Pure logic and reducers — no I/O, fully testable in isolation.    |
| **Backend**      | Request/Handler implementations, EF context, data I/O, reducers.  |
| **Frontend**     | Blazor components (DynamicComponent-compatible) and UI nodes.     |
| **Tests**        | Gherkin feature specs (`.feature`), scenario bindings, and fakes. |

---

## ?? Design Principles

* **Spec-First Development** — Every feature begins with a `.feature` file (Gherkin) and corresponding scenario tests.
* **Tri-Part Specs** — Each use case includes:

  1. Gherkin `.feature` file (`@VCHIP-XXXX` / `@UCXX` / `@SCXX` / `# UACXXX`)
  2. C# Scenario test class (`[Scenario(...)]`, `GivenAsync/WhenAsync`)
  3. One `[Then(...,"UACXXX")]` assertion per user acceptance criterion.
* **Plugin Autonomy** — A plugin can be cloned, tested, and released independently.
* **Stable Contracts** — Cross-plugin communication happens through Actions, Events, and NodeKinds only.
* **Agent Compatibility** — Each plugin can be reasoned about, rebuilt, and released by autonomous agents.

---

## ?? Workspace Integration

This plugin can be registered in your **WorkspaceBuilder** like so:

```csharp
var ws = WorkspaceBuilder.Create("LowlandTech", "Accounts", db);

ws.AddFeature("Registration", "Accounts")
  .Reducer("RegistrationReducer", r => r
      .Event("AccountRegistered", e => e
          .Param("AccountId", PropertyTypes.Guid)
          .Param("Email", PropertyTypes.String)))
  .EndReducer()
  .BuildAndSave();
```

This ensures deterministic seeding, GUID stability, and full integration with the FlowRegistry.

---

## ?? Testing & Specs

Run all BDD specifications with:

```bash
dotnet test
```

Each `.feature` file lives under:

```
src/plugins/lowlandtech.<feature>.tests/VCHIP-XXXX/
```

Each scenario test class mirrors its `.feature` name and uses your custom `[Scenario(...)]` attribute.

---

## ?? Plugin Lifecycle

| Stage                  | Description                                                                        |
| ---------------------- | ---------------------------------------------------------------------------------- |
| **Feature Definition** | Written in `.feature` (spec) and `.cs` (scenario class).                           |
| **Generation**         | Agents use `FeatureBuilder` / `WorkspaceBuilder` to generate code and test stubs.  |
| **Implementation**     | Humans or agents complete TODOs inside generated handlers or UI components.        |
| **Verification**       | `dotnet test` validates `[Then]` assertions for all UACs.                          |
| **Publication**        | The plugin is versioned and published as a NuGet package (or workspace submodule). |

---

## ?? Agent Tasks

Each plugin includes a `.agents/` folder with work items like:

```
.agents/
 ??? VCHIP-3010-FlowEventBus/
 ?   ??? Feature.feature
 ?   ??? Scenario.cs
 ?   ??? Workorder.md
```

Agents execute these workorders to complete or regenerate the plugin safely.

---

## ?? Example Feature Folder Layout

```
src/plugins/lowlandtech.accounts/
 ??? abstractions/
 ??? backend/
 ??? domain/
 ??? frontend/
 ??? tests/
 ?   ??? VCHIP-8010/
 ?       ??? EmailVerification.feature
 ?       ??? EmailVerificationScenario.cs
 ??? LowlandTech.Accounts.sln
 ??? README.md
 ??? .agents/
```

---

## ?? Coding Standards

* One `Given` per scenario; multiple `Then`s allowed.
* Use `[Specification("VCHIP-XXXX-UACXXX")]` for traceability.
* Never reference spec IDs in production code (only in tests).
* Prefer deterministic IDs (`GuidV5`) for nodes and features.
* UI components must expose `data-id="feature-name"` attributes for runtime discovery.

---

## ?? Tooling Notes

* **IDE:** Visual Studio / VS Code
* **Language:** .NET 9 (C# 13), Blazor (Server & MAUI)
* **Patterns:** Redux, Rx.NET, MediatR, FlowEngine, FeatureBuilder
* **Testing:** BDD (Gherkin), `[Scenario]`, `[Then]`
* **CI/CD:** GitHub Actions / local agents

---

## ?? Next Steps

* [ ] Write the `.feature` specification for your next flow.
* [ ] Generate code scaffolding via `FeatureBuilder`.
* [ ] Implement business logic or UI as required.
* [ ] Validate with `dotnet test`.
* [ ] Commit with spec ID in the message (e.g., `feat(VCHIP-8010): add email verification flow`).

---

## ?? License & Attribution

© 2025 **LowlandTech Foundry**
Part of the Vylyrian ecosystem.
Licensed under the LowlandTech Foundry Developer License (LT-FDL).

---

> ?? *“In the age of agentic development, every plugin is a mind — and every repo a world.”*
