# Feature Specification: Roast Calculator

**Feature Branch**: `001-roast-calculator`

**Created**: 2026-07-02

**Status**: Draft

**Input**: User description: "MeatyTimes is a simple cooking assistant app that helps home cooks calculate roasting instructions for joints of meat based on meat type, weight, and desired doneness. The initial version prioritises simplicity, speed, and accuracy over recipe management or meal planning features."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Calculate Roast Instructions (Priority: P1)

As a home cook who roasts joints of meat occasionally, I want to select a meat type, enter its weight, and choose doneness (where applicable), so that I receive clear step-by-step roasting instructions without searching online or consulting cookbooks.

**Why this priority**: This is the core value proposition of MeatyTimes. Without reliable instruction calculation, the product does not solve the stated problem.

**Independent Test**: Can be fully tested by entering meat type, weight, and doneness, then verifying the displayed instruction set includes temperatures, cooking durations, and resting time. Delivers immediate value as a standalone roasting reference.

**Acceptance Scenarios**:

1. **Given** a supported meat type, a valid weight, and a supported doneness level (where applicable), **When** the user requests instructions, **Then** the application displays initial oven temperature, initial cooking duration, secondary oven temperature (if required), remaining cooking duration, total cooking duration, and suggested resting time in a clear, scannable order.
2. **Given** a meat type that uses a single-temperature roast profile, **When** the user requests instructions, **Then** the application displays one oven temperature and one cooking duration (no secondary temperature step), plus resting time.
3. **Given** a meat type where doneness does not apply (e.g., poultry requiring a safe minimum), **When** the user requests instructions, **Then** doneness selection is not required and instructions reflect safe, fully-cooked guidance for that meat.
4. **Given** valid inputs, **When** the user requests instructions, **Then** results appear within a few seconds without navigating away from the main workflow.

---

### User Story 2 - Plan Cooking Start Time (Priority: P2)

As a home cook planning a meal, I want to enter the time I want to serve, so that the application tells me when to start cooking, when to change oven temperature, when to remove the meat, and when resting begins.

**Why this priority**: Serving-time planning removes a common source of stress for roast dinners and builds directly on the instruction output from Story 1, but the app still delivers core value without it.

**Independent Test**: Can be tested by entering meat parameters plus a target serving time, then verifying a backwards-calculated timeline with actionable milestones. Delivers meal-planning value independent of any save/history features.

**Acceptance Scenarios**:

1. **Given** valid roast parameters and a future serving time, **When** the user requests a cooking schedule, **Then** the application displays when cooking should begin, when to reduce temperature (if applicable), when to remove from the oven, and when resting should begin.
2. **Given** a roast profile with only one cooking temperature, **When** the user requests a cooking schedule, **Then** the timeline omits a temperature-change milestone and still shows start, removal, and resting times.
3. **Given** a serving time that is too soon to complete cooking and resting safely, **When** the user requests a cooking schedule, **Then** the application explains that the serving time is not achievable and indicates the earliest feasible serving time.

---

### User Story 3 - Use on Any Device (Priority: P3)

As a home cook using a phone, tablet, or desktop in the kitchen, I want the calculate-and-display workflow to work comfortably on my screen size, so that I can follow instructions while preparing food.

**Why this priority**: Roasting often happens in the kitchen on mobile devices, but responsive layout supports the primary flows rather than defining new capability.

**Independent Test**: Can be tested by completing the primary calculate flow on small, medium, and large viewports without horizontal scrolling or clipped instruction content.

**Acceptance Scenarios**:

1. **Given** a mobile viewport, **When** the user completes the calculate flow, **Then** all inputs and instruction outputs remain readable and operable without horizontal scrolling.
2. **Given** a desktop viewport, **When** the user views calculated instructions, **Then** the instruction steps remain in the same logical order and terminology as on mobile.

---

### Edge Cases

- What happens when weight is below the minimum supported for a meat type?
- What happens when weight exceeds the maximum supported for a meat type?
- How does the system handle non-numeric or empty weight input?
- How does the system respond when an unsupported meat type is requested?
- What happens when the user selects a serving time in the past?
- What happens when total cooking plus resting exceeds the time available before the serving time?
- How are instructions presented when a meat type has no secondary temperature reduction step?
- How does the system handle decimal weights (e.g., 1.5 kg)?
- What feedback is shown while instructions are being calculated (brief loading state)?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST allow users to select from a defined set of common roasting meat types (initial set: beef, lamb, pork, chicken, turkey).
- **FR-002**: System MUST allow users to enter joint weight in kilograms, supporting decimal values within per-meat-type supported ranges.
- **FR-003**: System MUST allow users to select desired doneness for meat types where doneness meaningfully affects cooking guidance (e.g., beef, lamb); for other meat types, the system MUST apply safe fully-cooked rules without requiring doneness selection.
- **FR-004**: System MUST calculate and display roasting instructions that include, at minimum: initial oven temperature, initial cooking duration, secondary oven temperature (when applicable), remaining cooking duration (when applicable), total cooking duration, and suggested resting time.
- **FR-005**: System MUST present instructions in a fixed, step-by-step order that home cooks can follow sequentially (oven settings → timing → temperature changes → resting).
- **FR-006**: System MUST produce deterministic results: the same meat type, weight, and doneness MUST always yield the same instruction set.
- **FR-007**: System MUST validate all user inputs before calculating and MUST provide clear, actionable error messages for invalid or out-of-range values without exposing internal errors.
- **FR-008**: System MUST allow users to optionally enter a target serving time and, when provided with valid roast parameters, calculate a backwards schedule showing when to start cooking, when to change temperature (if required), when to remove from the oven, and when resting begins.
- **FR-009**: System MUST warn users when a requested serving time cannot be met given the calculated cooking and resting durations, and MUST indicate the earliest achievable serving time.
- **FR-010**: System MUST display all temperatures in degrees Celsius and all durations in hours and minutes in a human-readable format.
- **FR-011**: System MUST complete instruction calculation and display results within 3 seconds under normal usage conditions.
- **FR-012**: System MUST support use on mobile, tablet, and desktop screen sizes for the primary calculate-and-display workflow.
- **FR-013**: System MUST NOT require user accounts, saved data, or internet search to complete the primary calculate flow (offline-capable calculation once the app is loaded is desirable but not mandatory for v0.1).

### Key Entities

- **Meat Type**: A supported category of roast (e.g., beef, lamb) with associated cooking rules, applicable doneness options, and supported weight range.
- **Roast Request**: User inputs comprising meat type, weight (kg), optional doneness, and optional target serving time.
- **Roasting Instructions**: The calculated output for a roast request—temperature profile, phased cooking durations, total duration, and resting guidance.
- **Cooking Schedule**: A time-based plan derived from roasting instructions and a target serving time, with milestones for start, temperature change, removal, and resting.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Users can select meat type, enter weight, choose doneness (where applicable), and receive complete roasting instructions in under 30 seconds end-to-end (including data entry).
- **SC-002**: 95% of instruction requests with valid inputs return results in under 3 seconds.
- **SC-003**: 90% of first-time users can complete the primary calculate flow without assistance or external reference material.
- **SC-004**: Users report that displayed instructions include all information they would otherwise search for online (temperature, timing, resting) in a single view.
- **SC-005**: When a serving time is provided, users can identify all four schedule milestones (start, temperature change if applicable, removal, resting start) without manual backwards calculation.
- **SC-006**: The calculate workflow is fully usable on viewports from 320px width upward without loss of critical instruction content.

## Assumptions

- Initial release uses metric weight (kilograms) and Celsius temperatures; imperial units are deferred to a future enhancement.
- The initial meat set covers the five most common UK home-roast joints: beef, lamb, pork, chicken, and turkey.
- Doneness options for applicable meats follow conventional home-cooking levels (e.g., rare, medium-rare, medium, well-done for beef).
- Cooking rules are based on documented, project-approved reference sources; provenance will be captured during planning, not in this specification.
- No user accounts, favourites, history, printable cards, kitchen timers, or recipe notes in v0.1—these are explicitly out of scope.
- The application is a web experience accessible on mobile, tablet, and desktop; native app distribution is out of scope.
- Food-safety minimum cooking guidance for poultry and pork takes precedence over user doneness preference where applicable.
- Single-user, anonymous usage; no personal data storage is required for v0.1.
