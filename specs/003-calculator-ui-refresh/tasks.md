# Tasks: Calculator UI Refresh

**Input**: Design documents from `/specs/003-calculator-ui-refresh/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/ui-contract.md, quickstart.md

**Tests**: Included — plan.md and quickstart.md specify bUnit component tests and `RoastDisplayFormatting` unit tests. No Playwright. Core tests unchanged.

**Organization**: Tasks grouped by user story for independent implementation and testing.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: User story label (US1–US6)
- Include exact file paths in descriptions

## Path Conventions

- **Source**: `src/MeatyTimes.Web/`
- **Tools**: `tools/tailwind/`
- **Tests**: `tests/MeatyTimes.Web.Tests/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Tailwind v4 standalone CLI pipeline (no Node/npm)

- [ ] T001 Add Tailwind v4 standalone CLI binary at `tools/tailwind/tailwindcss` (linux-x64, executable) and document OS replacement in `docs-internal/tech-stack.md`
- [ ] T002 Create `src/MeatyTimes.Web/Styles/app.css` with `@import "tailwindcss"`, `@source` for `Components/**/*.razor` and `Components/**/*.razor.css`, and `@theme` tokens matching the mockup palette (`#005f63` primary, light page, tinted results card, pale tip strip) per `specs/003-calculator-ui-refresh/visual-reference.jpg`
- [ ] T003 Add MSBuild `BeforeBuild` Exec target in `src/MeatyTimes.Web/MeatyTimes.Web.csproj` running `tailwindcss -i Styles/app.css -o wwwroot/css/app.css` from the Web project directory
- [ ] T004 [P] Add generated `src/MeatyTimes.Web/wwwroot/css/app.css` to `.gitignore` or commit a placeholder and link it from `src/MeatyTimes.Web/Components/App.razor` (remove MudBlazor CSS/JS links)

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Remove MudBlazor, add shared presentation primitives, and establish the new layout shell

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [ ] T005 Remove `MudBlazor` package reference from `src/MeatyTimes.Web/MeatyTimes.Web.csproj` and `Directory.Packages.props` if no longer referenced
- [ ] T006 Remove `AddMudServices()` from `src/MeatyTimes.Web/Program.cs`
- [ ] T007 [P] Remove MudBlazor `@using` and provider markup from `src/MeatyTimes.Web/Components/_Imports.razor` and `src/MeatyTimes.Web/Components/App.razor`
- [ ] T008 [P] Create closed-set `src/MeatyTimes.Web/Components/Icons/LucideIcon.razor` with inlined Lucide SVG paths (utensils, calculator, clock, book, info, thermometer, oven/flame, cloche, lightbulb, meat cues) and `aria-hidden="true"` on decorative icons per `specs/003-calculator-ui-refresh/research.md`
- [ ] T009 [P] Extend `src/MeatyTimes.Web/Components/Roast/RoastDisplayFormatting.cs` with `FormatTemperatureFahrenheit(int celsius)` (nearest 5 °F) and `FormatDurationCompact(int minutes)` per `specs/003-calculator-ui-refresh/data-model.md`
- [ ] T010 [P] Add unit tests for °F conversion and compact duration formatting in `tests/MeatyTimes.Web.Tests/RoastDisplayFormattingTests.cs`
- [ ] T011 Create `src/MeatyTimes.Web/Components/Layout/AppHeader.razor` with logo, MeatyTimes brand, and destination slots (Calculator active; History, Guides, About visible but non-working) per `specs/003-calculator-ui-refresh/contracts/ui-contract.md`
- [ ] T012 Rewrite `src/MeatyTimes.Web/Components/Layout/MainLayout.razor` to use `AppHeader`, light page background, and main content area — remove `MudAppBar`, `MudDrawer`, `MudLayout`, and `NavMenu` usage
- [ ] T013 [P] Delete or retire unused `src/MeatyTimes.Web/Components/Layout/NavMenu.razor` if fully replaced by `AppHeader.razor`
- [ ] T014 [P] Restyle `src/MeatyTimes.Web/Components/Layout/ReconnectModal.razor` and `src/MeatyTimes.Web/Components/Layout/ReconnectModal.razor.css` with Tailwind-compatible classes while preserving reconnect behaviour

**Checkpoint**: Foundation ready — MudBlazor removed, Tailwind builds, header shell in place

---

## Phase 3: User Story 1 - Recognise the New Calculator Layout (Priority: P1) 🎯 MVP

**Goal**: Default calculator page matches the mockup at a glance: branded header, “Your Roast” card, “Roasting Instructions” card, teal-and-light palette, no drawer/dark chrome.

**Independent Test**: Open `/` on desktop width before calculating — light page, header with Calculator active, two cards side by side, empty results chrome (heading only, no invented values). See quickstart Scenario 1.

### Implementation for User Story 1

- [ ] T015 [US1] Rewrite `src/MeatyTimes.Web/Components/Pages/RoastCalculator.razor` to a two-column Tailwind grid (`form ~2/5`, `results ~3/5` on `md+`) with always-visible “Your Roast” and “Roasting Instructions” card chrome per `specs/003-calculator-ui-refresh/contracts/ui-contract.md`
- [ ] T016 [P] [US1] Apply mockup card treatments (white input card, cooler tinted results card, rounded corners, generous padding, icon-led section titles) via Tailwind classes on `RoastInputForm` and `RoastResultsDisplay` shells in `src/MeatyTimes.Web/Components/Roast/RoastInputForm.razor` and `src/MeatyTimes.Web/Components/Roast/RoastResultsDisplay.razor`
- [ ] T017 [US1] Ensure empty results state shows “Roasting Instructions” heading and supporting line only — no invented temperatures, times, or input summary in `src/MeatyTimes.Web/Components/Roast/RoastResultsDisplay.razor` per `specs/003-calculator-ui-refresh/data-model.md` visibility table
- [ ] T018 [P] [US1] Remove page-level Mud typography (`MudText`, `MudGrid`, `MudItem`, `MudProgressLinear`, `MudAlert`) from `src/MeatyTimes.Web/Components/Pages/RoastCalculator.razor`; replace with Tailwind alert/loading patterns

**Checkpoint**: User Story 1 complete — page reads as the mockup before any calculation

---

## Phase 4: User Story 2 - Enter Roast Details in the New Form (Priority: P1)

**Goal**: Cooks enter meat type, weight (kg), doneness (when applicable), and request times via a restyled “Your Roast” card with native controls and one primary action.

**Independent Test**: Complete a valid calculation using only the form — instructions appear without leaving the page; invalid weight shows actionable error. See quickstart Scenarios 2 and 5.

### Tests for User Story 2 ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T019 [P] [US2] Remove `AddMudServices()` from `tests/MeatyTimes.Web.Tests/RoastResultsDisplayTests.cs` test fixture (and any new fixtures) per quickstart component test checklist
- [ ] T020 [P] [US2] Add bUnit tests for `RoastInputForm` markup: “Your Roast” title, native selects/inputs, `kg` suffix, “Calculate Times” button, doneness hidden for unsupported meat in `tests/MeatyTimes.Web.Tests/RoastInputFormTests.cs`

### Implementation for User Story 2

- [ ] T021 [US2] Rebuild `src/MeatyTimes.Web/Components/Roast/RoastInputForm.razor` with native `<select>` for meat and doneness, `<input type="number">` with visible `kg` suffix, helper text under fields, and full-width primary `<button>` labelled “Calculate Times” per `specs/003-calculator-ui-refresh/contracts/ui-contract.md`
- [ ] T022 [US2] Wire validation display (`WeightError`, `DonenessError`) to Tailwind field-level errors and an alert region in `src/MeatyTimes.Web/Components/Roast/RoastInputForm.razor` and `src/MeatyTimes.Web/Components/Pages/RoastCalculator.razor`
- [ ] T023 [US2] Preserve existing `OnCalculateRequested` callback and `RoastInputModel` meat/weight/doneness semantics in `src/MeatyTimes.Web/Components/Roast/RoastInputForm.razor` — no Core rule changes
- [ ] T024 [US2] Disable “Calculate Times” while `IsLoading` and show in-progress feedback in `src/MeatyTimes.Web/Components/Roast/RoastInputForm.razor` and `src/MeatyTimes.Web/Components/Pages/RoastCalculator.razor`

**Checkpoint**: User Story 2 complete — calculate workflow works in the new form

---

## Phase 5: User Story 3 - Read Restyled Instructions Without Losing Existing Guidance (Priority: P1)

**Goal**: Results card uses icon-led rows, °C primary / °F secondary, compact durations, input summary snapshot, all phases + rest + totals, and estimate disclaimer.

**Independent Test**: Calculate a multi-phase roast — summary, every phase, rest, totals, dual-unit temperatures, and disclaimer all visible; edit form without recalculating — summary unchanged. See quickstart Scenarios 2 and 3.

### Tests for User Story 3 ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T025 [P] [US3] Rewrite `tests/MeatyTimes.Web.Tests/RoastResultsDisplayTests.cs` without MudBlazor — assert summary labels/values, instruction heading, and empty snapshot behaviour per quickstart checklist
- [ ] T026 [P] [US3] Add test: each cooking phase renders as its own row (not collapsed) when `CookingResultDto` has multiple phases in `tests/MeatyTimes.Web.Tests/RoastResultsDisplayTests.cs`
- [ ] T027 [P] [US3] Add test: temperature rows show °C prominently and °F secondary via `RoastDisplayFormatting` in `tests/MeatyTimes.Web.Tests/RoastResultsDisplayTests.cs`

### Implementation for User Story 3

- [ ] T028 [US3] Rebuild `src/MeatyTimes.Web/Components/Roast/RoastResultsDisplay.razor` with icon-led instruction rows (one per `PhaseDto`), rest row, total cooking / total preparation rows, and estimate disclaimer per `specs/003-calculator-ui-refresh/contracts/ui-contract.md`
- [ ] T029 [US3] Render input summary (meat display name, `{0.0} kg`, conditional doneness) from snapshot `Input` + `Meats` above instruction rows in `src/MeatyTimes.Web/Components/Roast/RoastResultsDisplay.razor` — retain 002 semantics
- [ ] T030 [US3] Apply `RoastDisplayFormatting.FormatTemperatureFahrenheit` and `FormatDurationCompact` to prominent result values in `src/MeatyTimes.Web/Components/Roast/RoastResultsDisplay.razor`
- [ ] T031 [US3] Use primary teal treatment for prominent values and secondary text for °F equivalents in `src/MeatyTimes.Web/Components/Roast/RoastResultsDisplay.razor` per `specs/003-calculator-ui-refresh/spec.md` FR-002/FR-008

**Checkpoint**: User Stories 1–3 complete — full calculate-and-read path in the new design (MVP)

---

## Phase 6: User Story 4 - Optional Serve Time in the New Layout (Priority: P2)

**Goal**: Optional serve-at lives in “Your Roast”; single Calculate Times triggers schedule when a time is set; milestones or unachievable warning appear in results.

**Independent Test**: Calculate without serve-at → instructions only. Set future serve-at and calculate → milestones appear. Too-soon time → warning with earliest feasible time. See quickstart Scenario 4.

### Implementation for User Story 4

- [ ] T032 [US4] Add optional `ServingTime` (`DateTimeOffset?`) to `RoastInputForm.RoastInputModel` in `src/MeatyTimes.Web/Components/Roast/RoastInputForm.razor` per `specs/003-calculator-ui-refresh/data-model.md`
- [ ] T033 [US4] Add optional native `<input type="datetime-local">` with helper text to `src/MeatyTimes.Web/Components/Roast/RoastInputForm.razor` per mockup
- [ ] T034 [US4] Update `HandleCalculate` in `src/MeatyTimes.Web/Components/Pages/RoastCalculator.razor` to call `RoastService.PlanSchedule` when `ServingTime` is set (after successful `Calculate`), removing the separate Plan button flow
- [ ] T035 [US4] Restyle `src/MeatyTimes.Web/Components/Roast/ServeAtPlanner.razor` as schedule milestone display inside the results column (achievable milestones, unachievable warning, errors) — remove duplicate serve-at input controls
- [ ] T036 [US4] Preserve prior instruction snapshot on schedule failure in `src/MeatyTimes.Web/Components/Pages/RoastCalculator.razor` per `specs/003-calculator-ui-refresh/data-model.md` state transitions

**Checkpoint**: User Story 4 complete — serve-at relocated without capability loss

---

## Phase 7: User Story 5 - Use the New Layout in the Kitchen (Priority: P2)

**Goal**: Two-card layout stacks on narrow viewports; side-by-side on wide; no horizontal scroll; header remains usable.

**Independent Test**: Complete calculate at ~375px and desktop widths — cards stack/readable on mobile, side-by-side on desktop; Calculate Times reachable. See quickstart Scenario 6.

### Implementation for User Story 5

- [ ] T037 [US5] Add responsive Tailwind breakpoints so form and results stack (`flex-col` / single column) below `md` and sit side-by-side at `md+` in `src/MeatyTimes.Web/Components/Pages/RoastCalculator.razor`
- [ ] T038 [P] [US5] Ensure instruction rows and summary do not overflow horizontally on narrow viewports in `src/MeatyTimes.Web/Components/Roast/RoastResultsDisplay.razor`
- [ ] T039 [P] [US5] Make `AppHeader` destinations wrap or compress without hiding Calculator or the logo on narrow widths in `src/MeatyTimes.Web/Components/Layout/AppHeader.razor`

**Checkpoint**: User Story 5 complete — kitchen-friendly responsive layout

---

## Phase 8: User Story 6 - Trust the Page Chrome Without Fake Features (Priority: P3)

**Goal**: Header reserves future destinations without working History/Guides/About pages; no Save to History; generic tip strip.

**Independent Test**: Inspect header and results — only Calculator works; no save action; tip does not show misleading internal-temp ranges. See quickstart Scenario 7.

### Implementation for User Story 6

- [ ] T040 [US6] Render History, Guides, and About as non-interactive labelled items (not links to routes) in `src/MeatyTimes.Web/Components/Layout/AppHeader.razor` per `specs/003-calculator-ui-refresh/research.md` decision 8
- [ ] T041 [US6] Confirm `src/MeatyTimes.Web/Components/Roast/RoastResultsDisplay.razor` has no “Save to History” or equivalent control per FR-014
- [ ] T042 [US6] Add generic tip strip below the cards in `src/MeatyTimes.Web/Components/Pages/RoastCalculator.razor` (thermometer reminder; no unsourced °C internal-temp ranges) per `specs/003-calculator-ui-refresh/research.md` decision 9
- [ ] T043 [P] [US6] Restyle `src/MeatyTimes.Web/Components/Pages/Error.razor` and `src/MeatyTimes.Web/Components/Pages/NotFound.razor` without MudBlazor so error pages match the light chrome

**Checkpoint**: All user stories complete — chrome is honest and mockup-faithful

---

## Phase 9: Polish & Cross-Cutting Concerns

**Purpose**: Test suite hygiene, documentation, and manual validation

- [ ] T044 [P] Remove `MudBlazor` from `tests/MeatyTimes.Web.Tests/MeatyTimes.Web.Tests.csproj` if still referenced after test rewrites
- [ ] T045 [P] Update `docs-internal/tech-stack.md`, `README.md`, and `AGENTS.md` to describe Blazor Server + Tailwind v4 standalone CLI + Lucide (not MudBlazor)
- [ ] T046 Run `~/.dotnet/dotnet test MeatyTimes.slnx` and fix failures in `tests/MeatyTimes.Web.Tests/` and `tests/MeatyTimes.Core.Tests/`
- [ ] T047 Run quickstart.md validation Scenarios 1–7 (visual compare against `specs/003-calculator-ui-refresh/visual-reference.jpg`) and note results in `specs/003-calculator-ui-refresh/quickstart.md`
- [ ] T048 Verify no new abstractions beyond `LucideIcon`, `RoastDisplayFormatting`, and layout primitives violate Constitution Principle II in the feature plan Complexity Tracking table

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — start immediately
- **Foundational (Phase 2)**: Depends on Phase 1 — **blocks all user stories**
- **User Stories (Phases 3–8)**: Depend on Phase 2
  - US1–US3 (P1) form the MVP and should land sequentially or with US2/US3 after US1 card shells exist
  - US4 depends on US2 form and US3 results column
  - US5 can overlap US1–US3 once grid exists
  - US6 can overlap once header and results components exist
- **Polish (Phase 9)**: Depends on desired user stories being complete

### User Story Dependencies

| Story | Priority | Depends on | Independent test |
|-------|----------|------------|------------------|
| US1 | P1 | Phase 2 | Mockup layout/chrome before calculate |
| US2 | P1 | US1 card shell | Valid calculate from new form |
| US3 | P1 | US1 results shell | Multi-phase instructions + stable summary |
| US4 | P2 | US2, US3 | Serve-at optional + schedule milestones |
| US5 | P2 | US1 layout | Mobile stack + desktop columns |
| US6 | P3 | US1 header, US3 results | No fake features / tip strip |

### Within Each User Story

- Tests (where listed) before implementation
- Formatting helpers before results rows that consume them
- Form model changes before parent `RoastCalculator` schedule integration
- Core and `RoastService` remain unchanged

### Parallel Opportunities

- **Phase 1**: T004 parallel with T001–T003 after T002 exists
- **Phase 2**: T007, T008, T009, T010, T013, T014 parallel after T005–T006
- **US1**: T016, T018 parallel after T015
- **US2**: T019, T020 parallel; T021–T024 mostly sequential on same files
- **US3**: T025, T026, T027 parallel; then T028–T031 on `RoastResultsDisplay.razor`
- **US5**: T038, T039 parallel after T037
- **US6**: T043 parallel with T040–T042
- **Polish**: T044, T045 parallel

---

## Parallel Example: User Story 3

```bash
# Tests first (no MudBlazor):
Task T025: Rewrite tests/MeatyTimes.Web.Tests/RoastResultsDisplayTests.cs
Task T026: Multi-phase row test in tests/MeatyTimes.Web.Tests/RoastResultsDisplayTests.cs
Task T027: Dual-unit temperature test in tests/MeatyTimes.Web.Tests/RoastResultsDisplayTests.cs

# Then implementation on RoastResultsDisplay.razor:
Task T028 → T029 → T030 → T031
```

---

## Implementation Strategy

### MVP First (User Stories 1–3)

1. Complete Phase 1: Setup (Tailwind CLI)
2. Complete Phase 2: Foundational (remove Mud, header, formatting helpers)
3. Complete Phase 3: US1 — mockup layout recognisable
4. Complete Phase 4: US2 — form + calculate
5. Complete Phase 5: US3 — restyled instructions with full content
6. **STOP and VALIDATE** against quickstart Scenarios 1–3 and visual reference
7. Deploy/demo if ready

### Incremental Delivery

1. Setup + Foundational → tooling and shell ready
2. US1 → US2 → US3 → **MVP shipped**
3. US4 → serve-at in new layout
4. US5 → responsive polish
5. US6 → honest chrome + tip strip
6. Polish → docs, full test run, quickstart sign-off

### Parallel Team Strategy

After Phase 2:

- Developer A: US1 layout + US5 responsive grid
- Developer B: US2 form + US4 serve-at wiring
- Developer C: US3 results + US6 tip/header polish

---

## Notes

- [P] tasks = different files, no dependencies on incomplete tasks in the same phase
- [Story] label maps task to spec.md user stories for traceability
- Cooking maths, `MeatyTimes.Core`, and `RoastService` DTOs are out of scope for modification
- Playwright is explicitly not required for this feature
- Weight remains kilograms only; °F is display-only in Web
- Commit after each task or logical group; optional pre-hook: `/speckit-git-commit`
