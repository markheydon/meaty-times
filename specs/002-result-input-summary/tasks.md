# Tasks: Result Input Summary

**Input**: Design documents from `/specs/002-result-input-summary/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/ui-contract.md, quickstart.md

**Tests**: Included — plan.md specifies bUnit component tests for `RoastResultsDisplay` regression coverage. Presentation-only (no cooking-critical unit tests required).

**Organization**: Tasks grouped by user story for independent implementation and testing.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: User story label (US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Source**: `src/MeatyTimes.Web/`
- **Tests**: `tests/MeatyTimes.Web.Tests/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Create the Blazor component test project and wire it into the solution

- [x] T001 Create MeatyTimes.Web.Tests xUnit v3 project at tests/MeatyTimes.Web.Tests/MeatyTimes.Web.Tests.csproj targeting net10.0 with ProjectReference to src/MeatyTimes.Web/MeatyTimes.Web.csproj
- [x] T002 Add bUnit, MudBlazor, and Microsoft.NET.Test.Sdk package references in tests/MeatyTimes.Web.Tests/MeatyTimes.Web.Tests.csproj
- [x] T003 Add tests/MeatyTimes.Web.Tests/MeatyTimes.Web.Tests.csproj to MeatyTimes.slnx

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Shared presentation helpers used by summary rendering and form display

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [x] T004 [P] Extract shared doneness and weight display formatting (e.g., `WellDone` → "Well Done", weight `0.0 kg`) into src/MeatyTimes.Web/Components/Roast/RoastDisplayFormatting.cs
- [x] T005 [P] Update src/MeatyTimes.Web/Components/Roast/RoastInputForm.razor to use RoastDisplayFormatting instead of inline FormatDoneness

**Checkpoint**: Shared formatting ready — summary and form use consistent labels

---

## Phase 3: User Story 1 - See What a Result Is Based On (Priority: P1) 🎯 MVP

**Goal**: After a successful calculate, the results area shows a read-only summary of meat type, weight, and doneness (when applicable) directly above roasting instructions.

**Independent Test**: Calculate Beef 2.0 kg Medium — summary appears above instructions with correct labels and values. See quickstart Scenario 1 and Scenario 4.

### Tests for User Story 1 ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [x] T006 [P] [US1] Add bUnit test fixture with MudBlazor services in tests/MeatyTimes.Web.Tests/RoastResultsDisplayTests.cs
- [x] T007 [P] [US1] Add test: summary renders meat type display name, weight, and doneness when result and input provided in tests/MeatyTimes.Web.Tests/RoastResultsDisplayTests.cs
- [x] T008 [P] [US1] Add test: doneness row hidden when meat SupportsDoneness is false in tests/MeatyTimes.Web.Tests/RoastResultsDisplayTests.cs
- [x] T009 [P] [US1] Add test: component renders nothing when Result is null in tests/MeatyTimes.Web.Tests/RoastResultsDisplayTests.cs

### Implementation for User Story 1

- [x] T010 [US1] Add `Input` and `Meats` parameters to src/MeatyTimes.Web/Components/Roast/RoastResultsDisplay.razor per data-model.md
- [x] T011 [US1] Render labelled input summary block (Meat type, Weight (kg), conditional Doneness) above "Roasting instructions" in src/MeatyTimes.Web/Components/Roast/RoastResultsDisplay.razor per contracts/ui-contract.md
- [x] T012 [US1] Pass `_lastInput` and `_meats` to RoastResultsDisplay in src/MeatyTimes.Web/Components/Pages/RoastCalculator.razor

**Checkpoint**: User Story 1 complete — summary visible above instructions after successful calculate

---

## Phase 4: User Story 2 - Summary Stays Stable While Editing the Form (Priority: P1)

**Goal**: Input summary and instructions remain tied to the last successful calculation when the user edits form fields without recalculating; they update together on a new successful calculate; failed recalculates preserve prior results.

**Independent Test**: Calculate Beef 2.0 kg Medium, change form to Chicken 1.5 kg without Calculate — summary still shows Beef 2.0 kg Medium. See quickstart Scenarios 2, 3, and 5.

### Tests for User Story 2 ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [x] T013 [P] [US2] Add test: summary values derive from Input parameter not from mutable external state in tests/MeatyTimes.Web.Tests/RoastResultsDisplayTests.cs
- [x] T014 [P] [US2] Add test or page-level coverage: failed calculate preserves prior result display (document approach in test or manual quickstart Scenario 5) in tests/MeatyTimes.Web.Tests/RoastCalculatorPageTests.cs or tests/MeatyTimes.Web.Tests/RoastResultsDisplayTests.cs

### Implementation for User Story 2

- [x] T015 [US2] Update HandleCalculate catch block to preserve `_result` and `_lastInput` when a prior successful calculation exists in src/MeatyTimes.Web/Components/Pages/RoastCalculator.razor per research.md decision 5
- [x] T016 [US2] Ensure `_lastInput` is only updated on successful calculate (not on failed attempts) in src/MeatyTimes.Web/Components/Pages/RoastCalculator.razor
- [x] T017 [US2] Keep existing results and summary visible during loading state (do not hide on recalculate) in src/MeatyTimes.Web/Components/Pages/RoastCalculator.razor per contracts/ui-contract.md States table

**Checkpoint**: User Stories 1 and 2 complete — stale-form confusion fixed end-to-end

---

## Phase 5: User Story 3 - Read the Summary Quickly on Any Device (Priority: P2)

**Goal**: Input summary is easy to scan at a glance on mobile and desktop without horizontal scrolling.

**Independent Test**: Viewport 375×667, complete calculate — summary readable above instructions, no horizontal scroll. See quickstart Scenario 6.

### Implementation for User Story 3

- [x] T018 [P] [US3] Apply responsive summary layout (full-width stack, readable label/value spacing) in src/MeatyTimes.Web/Components/Roast/RoastResultsDisplay.razor
- [x] T019 [P] [US3] Verify summary does not overflow results column on narrow viewports in src/MeatyTimes.Web/Components/Pages/RoastCalculator.razor style block if needed

**Checkpoint**: All user stories independently functional across viewports

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Validation, solution hygiene, and constitution compliance

- [x] T020 [P] Run `dotnet test MeatyTimes.slnx` and fix any failures across tests/MeatyTimes.Web.Tests/, tests/MeatyTimes.Core.Tests/, and tests/MeatyTimes.Tests/
- [x] T021 Run quickstart.md validation Scenarios 1–6 manually or document results in specs/002-result-input-summary/quickstart.md
- [x] T022 Verify no new abstractions beyond RoastDisplayFormatting satisfy Constitution Principle VI (Pragmatic Simplicity)
- [x] T023 Confirm ServeAtPlanner in src/MeatyTimes.Web/Components/Roast/ServeAtPlanner.razor requires no duplicate input summary per spec assumptions

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — start immediately
- **Foundational (Phase 2)**: Depends on Phase 1 — BLOCKS user story implementation
- **User Story 1 (Phase 3)**: Depends on Phase 2 — MVP display
- **User Story 2 (Phase 4)**: Depends on Phase 3 — stability builds on summary rendering
- **User Story 3 (Phase 5)**: Depends on Phase 3 — styles the existing summary block
- **Polish (Phase 6)**: Depends on Phases 3–5 (minimum Phases 3–4 for MVP)

### User Story Dependencies

- **User Story 1 (P1)**: After Foundational — no dependency on US2/US3
- **User Story 2 (P1)**: After US1 — error-handling and stability require summary component to exist
- **User Story 3 (P2)**: After US1 — responsive styling on summary block; independent of US2

### Within Each User Story

- Tests MUST be written and FAIL before implementation
- US1: parameters → summary markup → parent wiring
- US2: tests → error-handling fix → loading-state behaviour
- US3: CSS/layout only

### Parallel Opportunities

- T004 and T005 can run in parallel (different files)
- T006–T009 (US1 tests) can run in parallel once T004 completes
- T013 and T014 (US2 tests) can run in parallel
- T018 and T019 (US3) can run in parallel
- T020 can run while manual validation (T021) is in progress

---

## Parallel Example: User Story 1

```bash
# Launch all US1 tests together (after T006 fixture exists):
Task: "Add test: summary renders meat type display name..." in tests/MeatyTimes.Web.Tests/RoastResultsDisplayTests.cs
Task: "Add test: doneness row hidden..." in tests/MeatyTimes.Web.Tests/RoastResultsDisplayTests.cs
Task: "Add test: component renders nothing when Result is null..." in tests/MeatyTimes.Web.Tests/RoastResultsDisplayTests.cs
```

---

## Implementation Strategy

### MVP First (User Stories 1 + 2)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1 (summary visible)
4. Complete Phase 4: User Story 2 (stability + error handling)
5. **STOP and VALIDATE**: quickstart Scenarios 1–5
6. Add Phase 5 (responsive polish) before release

### Incremental Delivery

1. Setup + Foundational → formatting helper ready
2. US1 → summary appears above instructions
3. US2 → form edits no longer confuse stale results (core fix)
4. US3 → mobile scannability polish
5. Polish → full test suite and quickstart sign-off

### Parallel Team Strategy

With two developers after Foundational:

- Developer A: US1 tests + implementation (T006–T012)
- Developer B: US2 error-handling (T015–T017) after T010 lands; US3 styling (T018–T019) after T011 lands

---

## Notes

- No API or MeatyTimes.Core changes — UI-only feature
- `_lastInput` snapshot already exists in RoastCalculator.razor; reuse rather than new state
- ServeAtPlanner unchanged — uses same `_lastInput` without duplicate summary
- Commit after each task or logical group
- Stop at US1+US2 checkpoint for MVP demo
