# Implementation Plan: Calculator UI Refresh

**Branch**: `003-calculator-ui-refresh` | **Date**: 2026-08-30 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/003-calculator-ui-refresh/spec.md`

**Note**: Visual migration only. No cooking-rule changes. No History, Guides, About, or Save to History.

## Summary

Replace the MudBlazor calculator chrome with the mockup’s light teal two-card layout, using **.NET 10 Blazor Server**, **.NET Aspire**, **Tailwind CSS v4 standalone CLI** (no Node), and **Lucide** icons. Native HTML controls replace Mud select/numeric/date widgets. Existing calculate, input summary, multi-phase instructions, rest, totals, validation, and serve-at remain; serve-at moves into “Your Roast” and runs from **Calculate Times** when a time is set. Chart.js is not introduced.

## Technical Context

**Language/Version**: C# / .NET 10 (Blazor Server, Interactive Server)

**Primary Dependencies**: ASP.NET Core; .NET Aspire (AppHost unchanged in behaviour); Tailwind v4 standalone CLI at `tools/tailwind/tailwindcss`; Lucide SVG subset; **no** MudBlazor, Fluent UI, Radzen, Syncfusion, Node, npm, or PostCSS

**Storage**: N/A (in-memory UI snapshot as today)

**Testing**: xUnit v3; built-in `Assert` only; NSubstitute only if isolation is required; bunit for Blazor components; no Playwright in this feature; AppHost modelling not tested

**Target Platform**: Web browsers via `MeatyTimes.Web` (mobile, tablet, desktop)

**Project Type**: UI refresh of existing Aspire-hosted Blazor web app

**Performance Goals**: Same single-page calculate path; CSS built at compile time; no extra runtime UI kit JS

**Constraints**:
- Tailwind: `tailwindcss -i Styles/app.css -o wwwroot/css/app.css`; `@source` all `.razor` and `.razor.css`; binary in `tools/tailwind/`
- UI is HTML + Tailwind; domain stays in `MeatyTimes.Core`; `RoastService` stays a thin facade
- Default look is the mockup light theme
- Dual °C/°F and compact durations are display-only in Web

**Scale/Scope**: `MeatyTimes.Web` layout/components/styles/tests + contributor docs. Core and AppHost resource graph unchanged.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Pre-Research | Post-Design |
|-----------|--------------|-------------|
| **I. Separation of Concerns** | PASS — restyle Web only; Core owns cooking | PASS — °F/duration helpers are presentation; `RoastService` still maps Core DTOs |
| **II. Architectural Discipline** | PASS — no new projects or kits beyond mandated Tailwind CLI | PASS — small `LucideIcon` + native fields; no extra abstraction layers |
| **III. Testability** | PASS — bunit rewrite; Core tests untouched | PASS — quickstart lists component/helper tests; no cooking-critical Core change so no new red-green Core suite |
| **IV. Explicit Error Handling** | PASS — keep existing error/empty/loading | PASS — native fields + alert regions; reconnect modal retained |
| **V. User-Facing Consistency** | PASS — same terms; instruction order preserved | PASS — ui-contract keeps 002 summary + phase order; mockup labels mapped |
| **VI. Security by Design** | PASS — no new persistence or auth | PASS — same validated inputs; no secrets |
| **VII. Traceability** | PASS — no rule changes | PASS — tip strip must not invent internal temperatures |
| **VIII. Code Quality** | PASS — remove Mud dead code; comment Tailwind target | PASS — document CLI path, theme tokens, icon vendor licence |

## Project Structure

### Documentation (this feature)

```text
specs/003-calculator-ui-refresh/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── ui-contract.md
├── visual-reference.jpg
└── tasks.md                 # /speckit-tasks — not created by /speckit-plan
```

### Source Code (repository root)

```text
tools/tailwind/
└── tailwindcss              # v4 standalone CLI (linux-x64 for CI/WSL)

src/MeatyTimes.Web/
├── Styles/
│   └── app.css              # @import "tailwindcss"; @source; @theme tokens
├── wwwroot/css/
│   └── app.css              # generated output
├── Components/
│   ├── App.razor            # drop Mud CSS/JS; link generated CSS
│   ├── _Imports.razor       # drop MudBlazor
│   ├── Icons/
│   │   └── LucideIcon.razor # closed Lucide set
│   ├── Layout/
│   │   ├── MainLayout.razor # header + main; no Mud providers/drawer
│   │   ├── AppHeader.razor  # logo + destinations
│   │   └── ReconnectModal.* # keep behaviour; Tailwind if needed
│   ├── Pages/
│   │   └── RoastCalculator.razor
│   └── Roast/
│       ├── RoastInputForm.razor
│       ├── RoastResultsDisplay.razor
│       ├── ServeAtPlanner.razor   # schedule display; input moves to form
│       └── RoastDisplayFormatting.cs
├── MeatyTimes.Web.csproj    # Tailwind BeforeBuild; remove MudBlazor
└── Program.cs               # remove AddMudServices

tests/MeatyTimes.Web.Tests/   # bunit without MudBlazor
tests/MeatyTimes.Core.Tests/ # unchanged

docs-internal/tech-stack.md
docs-internal/testing-standards.md  # confirm Playwright = full workflows only
README.md / AGENTS.md               # stack wording
```

**Structure Decision**: Stay in the existing Aspire solution. No new test or UI project. Tailwind CLI lives under `tools/`, not npm.

## Complexity Tracking

> No constitution violations. Tailwind CLI is a **project stack constraint**, not a new architectural layer inside the domain.

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| — | — | — |

## Phase 0 & Phase 1 Outputs

| Artifact | Path | Status |
|----------|------|--------|
| Research | [research.md](./research.md) | Complete |
| Data Model | [data-model.md](./data-model.md) | Complete |
| UI Contract | [contracts/ui-contract.md](./contracts/ui-contract.md) | Complete |
| Quickstart | [quickstart.md](./quickstart.md) | Complete |

## Implementation Notes (for `/speckit-tasks`)

1. **Tooling**: Add `tools/tailwind/tailwindcss` (v4 standalone), `Styles/app.css` with `@source` for all Razor files, MSBuild Exec `BeforeBuild`, link `wwwroot/css/app.css` in `App.razor`. No other CSS pipeline.

2. **Remove MudBlazor**: Package refs, `AddMudServices`, Mud CSS/JS, `_Imports`, theme/drawer. Rewrite every `Mud*` usage in calculator, layout, Error/NotFound as needed.

3. **Primitives**: Project-specific pieces only — header, cards, labelled fields, primary button, alert, instruction row, `LucideIcon`. HTML + Tailwind only.

4. **Form**: Meat, weight+kg, doneness, optional datetime-local, Calculate Times. Extend `RoastInputModel` with optional `ServingTime`. Parent calls `Calculate` then `PlanSchedule` when time is set.

5. **Results**: Card always shown; summary + phase rows + rest + totals + disclaimer after success; °F via `RoastDisplayFormatting`; no Save to History.

6. **Tests**: Drop Mud from Web.Tests; keep summary assertions; add formatting helper tests; keep `RoastServiceTests`. No Playwright. No AppHost tests.

7. **Docs**: tech-stack, README, AGENTS — Blazor Server + Tailwind v4 CLI + Lucide; not MudBlazor.

8. **Out of scope**: History/Guides/About pages, persistence, Chart.js, Core rule edits, new meats.

## Spec Reconciliation Notes

| Topic | Spec | Plan Decision |
|-------|------|----------------|
| MudBlazor | Implied replace to match mockup | Full removal; HTML + Tailwind |
| History / Guides / About | Design-forward, not implemented | Visible non-working header items |
| Save to History | Out of scope | Omit control |
| Serve-at | Keep, restyle into Your Roast | Optional field + same Calculate Times action |
| Instruction completeness | Do not drop phases | One row per phase + rest + totals |
| °F | Display equivalent | Web formatting, nearest 5 °F |
| Tip internal temps | Must not mislead | Generic tip; Core has no internal-temp field |
| Playwright | Full workflows only | Not added for this refresh |
| Chart.js | If an existing feature needs it | Not needed |
