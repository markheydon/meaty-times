# Implementation Plan: Roast Calculator

**Branch**: `001-roast-calculator` | **Date**: 2026-07-02 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/001-roast-calculator/spec.md` with planning scope from user (V1 meats, doneness levels, technical stack).

## Summary

Build a roast instruction calculator for home cooks: select meat type (beef, lamb, pork, chicken, gammon), enter weight in kg, choose doneness where applicable, and receive step-by-step oven temperatures, phased cooking durations, resting time, and total preparation time. Optionally enter a target serving time to receive a backwards cooking schedule.

Technical approach: extract cooking-critical logic into a shared `MeatyTimes.Core` library with JSON-defined cooking rules; call Core in-process from a MudBlazor Blazor Web App via `RoastService`; host with .NET Aspire as a single web container. V1 uses bundled JSON rules (not Azure Storage) to satisfy constitution Principle II.

## Technical Context

**Language/Version**: C# / .NET 10.0 (SDK 10.0.301)

**Primary Dependencies**: ASP.NET Core (Blazor Server), MudBlazor, .NET Aspire 13.x, OpenTelemetry via ServiceDefaults

**Storage**: JSON cooking-rules file bundled with `MeatyTimes.Core` (read-only at runtime). Azure Storage deferred to a future release when rules need remote updates without redeploy.

**Testing**: xUnit v3; unit tests for calculation engine in `MeatyTimes.Core.Tests`; component and service tests in `MeatyTimes.Web.Tests`

**Target Platform**: Web (mobile, tablet, desktop browsers); hosted via .NET Aspire AppHost locally and Azure Container Apps

**Project Type**: Single-container Aspire web application (Blazor UI + in-process Core)

**Performance Goals**: Instruction calculation completes in under 100 ms server-side; 95% of user requests return results in under 3 seconds end-to-end (per spec SC-002)

**Constraints**: Deterministic calculations; Celsius and kg only in V1; no user accounts or persistence; food-safety minimums for poultry and pork override doneness preference

**Scale/Scope**: 5 meat types, 1 cooking method (Traditional Roast) in V1, single primary UI page; serve-at scheduling as secondary flow

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Pre-Research | Post-Design |
|-----------|--------------|-------------|
| **I. Separation of Concerns** | PASS — Domain logic isolated in `MeatyTimes.Core` | PASS — `RoastCalculator` and `CookingRuleLoader` stay out of the UI |
| **II. Architectural Discipline** | PASS — JSON file over Azure Storage; single Core library over strategy/factory patterns; Traditional Roast only in V1 | PASS — Complexity Tracking documents deferred abstractions |
| **III. Testability** | PASS — Unit test plan for all 5 meats, weight boundaries, doneness variants, schedule calculation | PASS — `MeatyTimes.Core.Tests` with outcome-named tests per meat type |
| **IV. Explicit Error Handling** | PASS — Input validation at service boundary; user-facing errors without stack traces | PASS — Validation rules in data model; `RoastServiceException` for errors |
| **V. User-Facing Consistency** | PASS — Fixed instruction step order defined in contracts | PASS — UI contract specifies step order and terminology |
| **VI. Security by Design** | PASS — No secrets in JSON rules; no user data stored | PASS — Validation at the service boundary; no secrets in responses |
| **VII. Traceability** | PASS — Rules sourced from documented references in `research.md`; provenance field in rule JSON | PASS — Each rule entry includes `source` reference; deterministic engine |
| **VIII. Code Quality** | PASS — Calculation modules with required comments on rules | PASS — `RoastCalculator`, `CookingRuleLoader`, and rule JSON schema documented |

## Project Structure

### Documentation (this feature)

```text
specs/001-roast-calculator/
├── plan.md              # This file
├── research.md          # Phase 0 — technology and scope decisions
├── data-model.md        # Phase 1 — domain entities and validation
├── quickstart.md        # Phase 1 — validation scenarios and commands
├── contracts/           # Phase 1 — API and UI contracts
│   ├── roast-api.md
│   └── ui-contract.md
└── tasks.md             # Phase 2 (/speckit-tasks — not yet created)
```

### Source Code (repository root)

```text
src/
├── MeatyTimes.AppHost/           # Aspire orchestration (existing)
├── MeatyTimes.Core/              # Domain models, calculation engine, rule loading
│   ├── Domain/                   # MeatType, Doneness, RoastRequest, CookingResult, etc.
│   ├── Calculation/              # RoastCalculator, ScheduleCalculator
│   └── Rules/                    # cooking-rules.json + CookingRuleLoader
├── MeatyTimes.Web/               # Blazor UI (MudBlazor)
│   ├── Components/
│   │   ├── Pages/
│   │   │   └── RoastCalculator.razor
│   │   └── Roast/                # Input form, results display, schedule display
│   └── Services/                 # RoastService in-process facade + view DTOs
└── MeatyTimes.ServiceDefaults/   # OTel, health checks (existing)

tests/
├── MeatyTimes.Core.Tests/        # Unit tests for calculation engine
└── MeatyTimes.Web.Tests/         # Blazor component and RoastService tests
```

**Structure Decision**: `MeatyTimes.Core` is referenced by `MeatyTimes.Web` and `MeatyTimes.Core.Tests`. This keeps cooking logic testable without HTTP overhead and satisfies constitution Principle I (isolated domain modules) and Principle II (concrete types over interfaces). The Web project calls Core in-process via `RoastService`.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Separate `MeatyTimes.Core` project | Cooking logic must be unit-tested independently and shared by Web; constitution requires isolated domain modules | Inline logic in Web rejected — untestable without extraction; duplicate rules rejected — violates single source of truth |
| `CookingMethod` entity in data model (not user-selectable in V1) | Domain model anticipates Traditional/High Heat/Reverse Sear per user input; V1 hard-codes Traditional Roast | Removing entity entirely rejected — would require schema redesign when V2 adds method selection |
| Azure Storage deferred (user suggested it) | V1 rules are static, small (~5 meats), and change only via deploy | Azure Storage adds infrastructure, secrets, and latency for no V1 user value (Principle II) |

## Phase 0 & Phase 1 Outputs

| Artifact | Path | Status |
|----------|------|--------|
| Research | [research.md](./research.md) | Complete |
| Data Model | [data-model.md](./data-model.md) | Complete |
| API Contract | [contracts/roast-api.md](./contracts/roast-api.md) | Complete (in-process `RoastService`; HTTP API removed) |
| UI Contract | [contracts/ui-contract.md](./contracts/ui-contract.md) | Complete |
| Quickstart | [quickstart.md](./quickstart.md) | Complete |

## Spec Reconciliation Notes

The planning input refines the specification in these ways (documented, not blocking):

| Topic | Spec | V1 Plan Decision |
|-------|------|------------------|
| Meat types | beef, lamb, pork, chicken, **turkey** | beef, lamb, pork, chicken, **gammon** |
| Doneness levels | rare, medium-rare, medium, well-done | **rare, medium, well-done** (no medium-rare) |
| Cooking methods | Not specified | **Traditional Roast only** (internal default) |
| Rule storage | Not specified | **Bundled JSON** (Azure Storage deferred) |

Serve-at scheduling (Spec P2) remains in V1 scope.
