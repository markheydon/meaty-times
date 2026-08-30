# Implementation Plan: Result Input Summary

**Branch**: `002-result-input-summary` | **Date**: 2026-07-04 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/002-result-input-summary/spec.md`

## Summary

When roasting instructions are displayed, show a read-only summary of the inputs used for that calculation (meat type, weight, doneness when applicable) directly above the instruction details. The summary binds to the last successful calculation snapshot (`_lastInput`), not live form values, so users can edit the form without mistaking stale instructions for their current selection.

Technical approach: extend `RoastResultsDisplay` with input summary rendering; pass `_lastInput` and `_meats` from `RoastCalculator.razor`; adjust error handling so failed recalculates preserve prior successful results. No API or domain changes.

## Technical Context

**Language/Version**: C# / .NET 10.0

**Primary Dependencies**: Blazor Server (interactive), MudBlazor, existing `RoastService` view DTOs

**Storage**: N/A (client-side snapshot of last successful input in component state)

**Testing**: xUnit v3; `MeatyTimes.Web.Tests` with bUnit for `RoastResultsDisplay`

**Target Platform**: Web browsers via MeatyTimes.Web (mobile, tablet, desktop)

**Project Type**: UI enhancement to existing single-container web application

**Performance Goals**: No measurable latency impact; summary derived synchronously from in-memory state

**Constraints**: Summary must use form-consistent terminology; no API contract changes; failed recalculate must not clear prior successful results per spec edge cases

**Scale/Scope**: Single component update, one parent wiring change, optional error-handling fix, ~4 component tests

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Pre-Research | Post-Design |
|-----------|--------------|-------------|
| **I. Code Quality** | PASS — UI-only; label formatting colocated in results component or shared static helper | PASS — `CalculationInputSummary` derivation documented in data model; no domain logic moved |
| **II. Testing Standards** | PASS — Presentation tests planned; no cooking calculation changes | PASS — bUnit component tests for summary visibility, doneness conditional, display names |
| **III. Consistent UX** | PASS — Form labels reused; summary above instructions in fixed order | PASS — UI contract defines layout, states, and terminology |
| **IV. Security** | PASS — No new inputs or endpoints; summary displays already-validated submitted values | PASS — No secrets; read-only display of prior successful input |
| **V. Cooking Accuracy** | PASS — Calculation engine untouched; instructions unchanged | PASS — No rule or algorithm changes |
| **VI. Pragmatic Simplicity** | PASS — Reuses `_lastInput` snapshot; no new API fields or services | PASS — Extends existing component; bUnit justified for Blazor markup regression |

## Project Structure

### Documentation (this feature)

```text
specs/002-result-input-summary/
├── plan.md              # This file
├── research.md          # Phase 0 — design decisions
├── data-model.md        # Phase 1 — presentation model
├── quickstart.md        # Phase 1 — validation scenarios
├── contracts/
│   └── ui-contract.md   # Phase 1 — updated UI behaviour
└── tasks.md             # Phase 2 (/speckit-tasks — not yet created)
```

### Source Code (repository root)

```text
src/
├── MeatyTimes.Web/
│   ├── Components/
│   │   ├── Pages/
│   │   │   └── RoastCalculator.razor      # Pass Input + Meats; preserve result on error
│   │   └── Roast/
│   │       └── RoastResultsDisplay.razor  # Add input summary block above instructions
│   └── Services/
│       └── RoastService.cs                # Unchanged for this feature

tests/
├── MeatyTimes.Web.Tests/                  # bUnit component tests
│   └── RoastResultsDisplayTests.cs
└── MeatyTimes.Core.Tests/                 # Unchanged
```

**Structure Decision**: All changes confined to `MeatyTimes.Web` plus component tests. No changes to `MeatyTimes.Core`. Parent page already owns `_lastInput` — minimal state wiring.

## Complexity Tracking

> No constitution violations. Table intentionally empty.

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

1. **RoastResultsDisplay**: Add `Input` and `Meats` parameters; render summary block when both `Result` and `Input` are set; resolve `DisplayName` and `SupportsDoneness` from catalog; format weight and doneness per [data-model.md](./data-model.md).

2. **RoastCalculator.razor**: Pass `_lastInput` and `_meats` to `RoastResultsDisplay`. In `HandleCalculate` catch block, only set `_result = null` when there was no prior successful result (or remove unconditional clear entirely and only update on success).

3. **Doneness formatting**: Reuse same mapping as `RoastInputForm.FormatDoneness` — extract to shared static helper if duplication would otherwise occur (single small helper acceptable per Principle VI).

4. **Tests**: Create `MeatyTimes.Web.Tests` with bUnit + MudBlazor test harness; cover scenarios in [quickstart.md](./quickstart.md) component test checklist.

5. **No API contract**: Skip `contracts/roast-api.md` — no API surface changes.

## Spec Reconciliation Notes

| Topic | Spec | Plan Decision |
|-------|------|---------------|
| Data source | Last successful calculation inputs | `_lastInput` + meat catalog for display names |
| Failed recalculate | Keep prior summary/instructions | Fix current behaviour that clears `_result` on error |
| Serve-at section | Unchanged | No duplicate summary in `ServeAtPlanner` |
| API changes | Not required | UI-only feature |
