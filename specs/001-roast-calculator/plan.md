# Implementation Plan: Roast Calculator

**Branch**: `001-roast-calculator` | **Date**: 2026-07-02 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/001-roast-calculator/spec.md` with planning scope from user (V1 meats, doneness levels, technical stack).

## Summary

Build a roast instruction calculator for home cooks: select meat type (beef, lamb, pork, chicken, gammon), enter weight in kg, choose doneness where applicable, and receive step-by-step oven temperatures, phased cooking durations, resting time, and total preparation time. Optionally enter a target serving time to receive a backwards cooking schedule.

Technical approach: extract cooking-critical logic into a shared `MeatyTimes.Core` library with JSON-defined cooking rules; expose calculation via ASP.NET Core minimal APIs; present results in a MudBlazor Blazor Web App; host with .NET Aspire. V1 uses bundled JSON rules (not Azure Storage) to satisfy constitution Principle VI.

## Technical Context

**Language/Version**: C# / .NET 10.0 (SDK 10.0.301)

**Primary Dependencies**: ASP.NET Core (minimal APIs), Blazor Server (interactive), MudBlazor, .NET Aspire 13.x, OpenTelemetry via ServiceDefaults

**Storage**: JSON cooking-rules file bundled with `MeatyTimes.Core` (read-only at runtime). Azure Storage deferred to a future release when rules need remote updates without redeploy.

**Testing**: xUnit v3, Aspire.Hosting.Testing (integration), unit tests for calculation engine in dedicated test project

**Target Platform**: Web (mobile, tablet, desktop browsers); hosted via .NET Aspire AppHost locally and container/cloud deployment later

**Project Type**: Distributed web application (Blazor frontend + API backend + AppHost orchestration)

**Performance Goals**: Instruction calculation completes in under 100 ms server-side; 95% of user requests return results in under 3 seconds end-to-end (per spec SC-002)

**Constraints**: Deterministic calculations; Celsius and kg only in V1; no user accounts or persistence; food-safety minimums for poultry and pork override doneness preference

**Scale/Scope**: 5 meat types, 1 cooking method (Traditional Roast) in V1, single primary UI page plus API endpoints; serve-at scheduling as secondary flow

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Pre-Research | Post-Design |
|-----------|--------------|-------------|
| **I. Code Quality** | PASS — Domain logic isolated in `MeatyTimes.Core`; calculation modules with required comments on rules | PASS — `RoastCalculator`, `CookingRuleLoader`, and rule JSON schema documented |
| **II. Testing Standards** | PASS — Unit test plan for all 5 meats, weight boundaries, doneness variants, schedule calculation | PASS — `MeatyTimes.Core.Tests` with outcome-named tests per meat type |
| **III. Consistent UX** | PASS — Fixed instruction step order defined in contracts; MudBlazor components for consistent validation/errors | PASS — UI contract specifies step order and terminology |
| **IV. Security** | PASS — Input validation at API boundary; no secrets in JSON rules; no user data stored | PASS — Validation rules in data model; ProblemDetails for errors |
| **V. Cooking Accuracy** | PASS — Rules sourced from documented references in `research.md`; provenance field in rule JSON | PASS — Each rule entry includes `source` reference; deterministic engine |
| **VI. Pragmatic Simplicity** | PASS — JSON file over Azure Storage; single Core library over strategy/factory patterns; Traditional Roast only in V1 | PASS — Complexity Tracking documents deferred abstractions |

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
├── MeatyTimes.ApiService/        # REST API for roast calculation
│   ├── Endpoints/                # Minimal API route groups
│   └── Program.cs
├── MeatyTimes.Core/              # NEW — domain models, calculation engine, rule loading
│   ├── Domain/                   # MeatType, Doneness, RoastRequest, CookingResult, etc.
│   ├── Calculation/              # RoastCalculator, ScheduleCalculator
│   └── Rules/                    # cooking-rules.json + CookingRuleLoader
├── MeatyTimes.Web/               # Blazor UI (MudBlazor)
│   ├── Components/
│   │   ├── Pages/
│   │   │   └── RoastCalculator.razor
│   │   └── Roast/                # Input form, results display, schedule display
│   └── Services/                 # RoastApiClient HTTP wrapper
└── MeatyTimes.ServiceDefaults/   # OTel, health checks (existing)

tests/
├── MeatyTimes.Tests/             # Aspire integration tests (existing, extended)
└── MeatyTimes.Core.Tests/        # NEW — unit tests for calculation engine
```

**Structure Decision**: Add `MeatyTimes.Core` as a class library referenced by both `ApiService` and `Core.Tests`. This keeps cooking logic testable without HTTP overhead and satisfies constitution Principle I (isolated domain modules) and Principle VI (concrete types over interfaces). The Web project calls the API via typed HTTP client (existing Aspire service-discovery pattern).

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Separate `MeatyTimes.Core` project (4th src project) | Cooking logic must be unit-tested independently and shared by API; constitution requires isolated domain modules | Inline logic in ApiService rejected — untestable without HTTP; duplicate in Web rejected — violates single source of truth |
| `CookingMethod` entity in data model (not user-selectable in V1) | Domain model anticipates Traditional/High Heat/Reverse Sear per user input; V1 hard-codes Traditional Roast | Removing entity entirely rejected — would require schema redesign when V2 adds method selection |
| Azure Storage deferred (user suggested it) | V1 rules are static, small (~5 meats), and change only via deploy | Azure Storage adds infrastructure, secrets, and latency for no V1 user value (Principle VI) |

## Phase 0 & Phase 1 Outputs

| Artifact | Path | Status |
|----------|------|--------|
| Research | [research.md](./research.md) | Complete |
| Data Model | [data-model.md](./data-model.md) | Complete |
| API Contract | [contracts/roast-api.md](./contracts/roast-api.md) | Complete |
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
