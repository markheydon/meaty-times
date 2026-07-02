# Tasks: Roast Calculator

**Input**: Design documents from `/specs/001-roast-calculator/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Tests**: Included — constitution Principle II (NON-NEGOTIABLE) requires unit tests for cooking-critical calculation logic. Tests MUST be written first and fail before implementation (red-green-refactor).

**Organization**: Tasks grouped by user story for independent implementation and testing.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: User story label (US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Source**: `src/` (MeatyTimes.Core, ApiService, Web, AppHost, ServiceDefaults)
- **Tests**: `tests/MeatyTimes.Core.Tests/`, `tests/MeatyTimes.Tests/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Create new projects and wire them into the solution

- [ ] T001 Create MeatyTimes.Core class library at src/MeatyTimes.Core/MeatyTimes.Core.csproj targeting net10.0
- [ ] T002 Create MeatyTimes.Core.Tests xUnit v3 project at tests/MeatyTimes.Core.Tests/MeatyTimes.Core.Tests.csproj with reference to src/MeatyTimes.Core/MeatyTimes.Core.csproj
- [ ] T003 Add MeatyTimes.Core and MeatyTimes.Core.Tests to MeatyTimes.slnx
- [ ] T004 [P] Add ProjectReference to MeatyTimes.Core in src/MeatyTimes.ApiService/MeatyTimes.ApiService.csproj
- [ ] T005 [P] Configure src/MeatyTimes.Core/Rules/cooking-rules.json as embedded resource in src/MeatyTimes.Core/MeatyTimes.Core.csproj

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Domain types, cooking rules, and API scaffolding that ALL user stories depend on

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [ ] T006 [P] Create domain enums (MeatTypeId, Doneness, CookingMethod) in src/MeatyTimes.Core/Domain/
- [ ] T007 [P] Create domain records (RoastRequest, CookingPhase, CookingResult, CookingSchedule, MeatTypeInfo, CookingRule) in src/MeatyTimes.Core/Domain/
- [ ] T008 [P] Create RoastValidationException and validation helpers in src/MeatyTimes.Core/Domain/RoastValidationException.cs
- [ ] T009 Create cooking-rules.json with all 5 meats (beef, lamb, pork, chicken, gammon), documented source fields, and TraditionalRoast profiles in src/MeatyTimes.Core/Rules/cooking-rules.json
- [ ] T010 Implement CookingRuleLoader to deserialize and validate rules at startup in src/MeatyTimes.Core/Rules/CookingRuleLoader.cs
- [ ] T011 Register CookingRuleLoader as singleton and add ProblemDetails error handling in src/MeatyTimes.ApiService/Program.cs
- [ ] T012 Create roast endpoint scaffold with route group prefix /api in src/MeatyTimes.ApiService/Endpoints/RoastEndpoints.cs

**Checkpoint**: Foundation ready — domain types, rules, and API scaffold in place

---

## Phase 3: User Story 1 - Calculate Roast Instructions (Priority: P1) 🎯 MVP

**Goal**: Home cook selects meat type, weight, and doneness (where applicable) and receives step-by-step roasting instructions with temperatures, phased durations, resting time, and total preparation time.

**Independent Test**: Open `/`, select Beef, enter 2.0 kg, choose Medium, click Calculate — results show initial temperature, phased durations, total cooking, resting, and total preparation within 3 seconds. See quickstart Scenario 1.

### Tests for User Story 1 ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T013 [P] [US1] Add beef and lamb doneness unit tests (rare/medium/well-done, two-phase profiles) in tests/MeatyTimes.Core.Tests/RoastCalculatorBeefLambTests.cs
- [ ] T014 [P] [US1] Add pork, chicken, and gammon unit tests (no doneness, safe-cook profiles) in tests/MeatyTimes.Core.Tests/RoastCalculatorOtherMeatsTests.cs
- [ ] T015 [P] [US1] Add weight boundary and validation unit tests in tests/MeatyTimes.Core.Tests/RoastCalculatorValidationTests.cs
- [ ] T016 [P] [US1] Add determinism unit test (same inputs → same outputs) in tests/MeatyTimes.Core.Tests/RoastCalculatorDeterminismTests.cs

### Implementation for User Story 1

- [ ] T017 [US1] Implement RoastCalculator with phased temperature logic, doneness adjustments, resting calculation, and required code comments in src/MeatyTimes.Core/Calculation/RoastCalculator.cs
- [ ] T018 [US1] Implement GET /api/meats endpoint returning meat metadata and doneness options in src/MeatyTimes.ApiService/Endpoints/RoastEndpoints.cs
- [ ] T019 [US1] Implement POST /api/roast/calculate endpoint with input validation and ProblemDetails errors in src/MeatyTimes.ApiService/Endpoints/RoastEndpoints.cs
- [ ] T020 [P] [US1] Create RoastApiClient with GetMeatsAsync and CalculateAsync methods in src/MeatyTimes.Web/Services/RoastApiClient.cs
- [ ] T021 [P] [US1] Create RoastInputForm component (meat select, weight field, conditional doneness) in src/MeatyTimes.Web/Components/Roast/RoastInputForm.razor
- [ ] T022 [P] [US1] Create RoastResultsDisplay component (ordered steps, formatted durations) in src/MeatyTimes.Web/Components/Roast/RoastResultsDisplay.razor
- [ ] T023 [US1] Create RoastCalculator page wiring form, calculate action, loading state, and error display in src/MeatyTimes.Web/Components/Pages/RoastCalculator.razor
- [ ] T024 [US1] Register RoastApiClient HttpClient and map roast page as home route in src/MeatyTimes.Web/Program.cs
- [ ] T025 [US1] Add API integration test for POST /api/roast/calculate in tests/MeatyTimes.Tests/RoastApiTests.cs

**Checkpoint**: User Story 1 fully functional — calculate flow works end-to-end via UI and API

---

## Phase 4: User Story 2 - Plan Cooking Start Time (Priority: P2)

**Goal**: Home cook enters a target serving time and receives a backwards schedule with start cooking, temperature change, removal, and resting milestones.

**Independent Test**: After calculating beef 2 kg medium, enter a serving time 2+ hours ahead, click Plan — schedule shows all milestones in correct order. See quickstart Scenario 5.

### Tests for User Story 2 ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T026 [P] [US2] Add achievable schedule unit tests in tests/MeatyTimes.Core.Tests/ScheduleCalculatorTests.cs
- [ ] T027 [P] [US2] Add unachievable schedule and earliest-serving-time unit tests in tests/MeatyTimes.Core.Tests/ScheduleCalculatorUnachievableTests.cs

### Implementation for User Story 2

- [ ] T028 [US2] Implement ScheduleCalculator with backwards time arithmetic from CookingResult in src/MeatyTimes.Core/Calculation/ScheduleCalculator.cs
- [ ] T029 [US2] Implement POST /api/roast/schedule endpoint with future-time validation in src/MeatyTimes.ApiService/Endpoints/RoastEndpoints.cs
- [ ] T030 [P] [US2] Add PlanScheduleAsync method to src/MeatyTimes.Web/Services/RoastApiClient.cs
- [ ] T031 [US2] Create ServeAtPlanner component (datetime input, plan button, milestone display, unachievable warning) in src/MeatyTimes.Web/Components/Roast/ServeAtPlanner.razor
- [ ] T032 [US2] Integrate ServeAtPlanner into src/MeatyTimes.Web/Components/Pages/RoastCalculator.razor below results section

**Checkpoint**: User Stories 1 and 2 both work independently

---

## Phase 5: User Story 3 - Use on Any Device (Priority: P3)

**Goal**: Calculate-and-display workflow is comfortable on mobile, tablet, and desktop without horizontal scrolling or clipped content.

**Independent Test**: Set viewport to 375×667, complete calculate flow — all inputs and results readable without horizontal scroll. See quickstart Scenario 7.

### Implementation for User Story 3

- [ ] T033 [P] [US3] Apply responsive MudGrid single-column layout for viewports under 600px in src/MeatyTimes.Web/Components/Pages/RoastCalculator.razor
- [ ] T034 [P] [US3] Ensure full-width inputs and readable typography on small screens in src/MeatyTimes.Web/Components/Roast/RoastInputForm.razor
- [ ] T035 [P] [US3] Ensure results and schedule sections wrap without overflow in src/MeatyTimes.Web/Components/Roast/RoastResultsDisplay.razor and src/MeatyTimes.Web/Components/Roast/ServeAtPlanner.razor
- [ ] T036 [US3] Add accessibility labels and ErrorText on all form fields per contracts/ui-contract.md across src/MeatyTimes.Web/Components/Roast/

**Checkpoint**: All three user stories independently functional across device sizes

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Cleanup, validation, and constitution compliance

- [ ] T037 [P] Remove starter Counter and Weather pages and nav links from src/MeatyTimes.Web/Components/Layout/NavMenu.razor
- [ ] T038 [P] Remove unused WeatherApiClient and sample API endpoints from src/MeatyTimes.Web/Program.cs and src/MeatyTimes.ApiService/Program.cs
- [ ] T039 Update README.md with MeatyTimes purpose, run commands, and link to specs/001-roast-calculator/quickstart.md
- [ ] T040 Run all quickstart.md validation scenarios and fix any failures
- [ ] T041 Verify new abstractions satisfy Constitution Principle VI (no interfaces/factories beyond justified Core library split)
- [ ] T042 Confirm every rule in src/MeatyTimes.Core/Rules/cooking-rules.json has a documented source reference per Principle V

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — start immediately
- **Foundational (Phase 2)**: Depends on Phase 1 — **BLOCKS all user stories**
- **User Story 1 (Phase 3)**: Depends on Phase 2
- **User Story 2 (Phase 4)**: Depends on Phase 2 and T017 (RoastCalculator / CookingResult)
- **User Story 3 (Phase 5)**: Depends on Phase 3 (UI components exist to make responsive)
- **Polish (Phase 6)**: Depends on desired user stories being complete

### User Story Dependencies

- **US1 (P1)**: Starts after Foundational — no dependency on US2/US3
- **US2 (P2)**: Starts after Foundational + RoastCalculator (T017) — builds on US1 results but API/UI independently testable
- **US3 (P3)**: Starts after US1 UI exists (T021–T023) — layout polish only

### Within Each User Story

- Tests written and failing before implementation
- Core calculation before API endpoints
- API endpoints before Web client
- Web client before page integration

### Parallel Opportunities

- **Phase 1**: T004 and T005 in parallel after T001–T003
- **Phase 2**: T006, T007, T008 in parallel; T009 after T007
- **Phase 3 tests**: T013, T014, T015, T016 all in parallel
- **Phase 3 UI**: T020, T021, T022 in parallel after T019
- **Phase 4 tests**: T026 and T027 in parallel
- **Phase 5**: T033, T034, T035 in parallel
- **Phase 6**: T037 and T038 in parallel

---

## Parallel Example: User Story 1

```bash
# Launch all unit tests together (must fail before T017):
Task T013: "tests/MeatyTimes.Core.Tests/RoastCalculatorBeefLambTests.cs"
Task T014: "tests/MeatyTimes.Core.Tests/RoastCalculatorOtherMeatsTests.cs"
Task T015: "tests/MeatyTimes.Core.Tests/RoastCalculatorValidationTests.cs"
Task T016: "tests/MeatyTimes.Core.Tests/RoastCalculatorDeterminismTests.cs"

# Launch UI components in parallel (after API endpoints):
Task T021: "src/MeatyTimes.Web/Components/Roast/RoastInputForm.razor"
Task T022: "src/MeatyTimes.Web/Components/Roast/RoastResultsDisplay.razor"
Task T020: "src/MeatyTimes.Web/Services/RoastApiClient.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup (T001–T005)
2. Complete Phase 2: Foundational (T006–T012)
3. Complete Phase 3: User Story 1 (T013–T025)
4. **STOP and VALIDATE**: Run quickstart Scenarios 1–4
5. Demo/deploy MVP

### Incremental Delivery

1. Setup + Foundational → foundation ready
2. User Story 1 → test independently → **MVP**
3. User Story 2 → test independently → serve-at planning added
4. User Story 3 → test independently → mobile-ready
5. Polish → production-ready

### Parallel Team Strategy

With multiple developers after Foundational:

- **Developer A**: US1 calculation + API (T013–T019, T025)
- **Developer B**: US1 UI (T020–T024, after T019)
- **Developer C**: US2 (T026–T032, after T017)

---

## Notes

- V1 meats: beef, lamb, pork, chicken, gammon (gammon replaces turkey per plan.md)
- Doneness: Rare, Medium, WellDone for beef/lamb only
- All cooking logic lives in MeatyTimes.Core — never duplicate in Web or ApiService
- Code comments required on all calculation logic per constitution Principle I
- Commit after each task or logical group
