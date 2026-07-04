# Feature Specification: Result Input Summary

**Feature Branch**: `002-result-input-summary`

**Created**: 2026-07-04

**Status**: Draft

**Input**: User description: "an improvement is needed to the calculation UI when the result has been shown. Basically if the user starts to change the selection it can quickly confuse as the result shown no longer matches what the user entered. So it would be better to show the selection the calculation relates to above the calculation. I.e. just repeat what the user selected before outputting the cooking calculation details. Then it doesn't matter what the user had on the left in the form fields it's still clear what the calculation relates to."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - See What a Result Is Based On (Priority: P1)

As a home cook who has calculated roasting instructions, I want the displayed result to show the meat type, weight, and doneness (when applicable) that were used for that calculation, so that I always know which joint the instructions refer to even if I start changing the form.

**Why this priority**: This directly fixes the confusion described in the feature request. Without a visible input summary, users can misread stale instructions and follow guidance for the wrong joint.

**Independent Test**: Can be fully tested by calculating instructions, then changing one or more form fields without recalculating, and verifying the result area still shows the original selection above the instructions.

**Acceptance Scenarios**:

1. **Given** a successful calculation has been performed, **When** roasting instructions are displayed, **Then** a concise summary of the inputs used for that calculation appears above the instruction details, including meat type and weight.
2. **Given** the calculated meat type supports doneness selection, **When** roasting instructions are displayed, **Then** the input summary includes the doneness level used for that calculation.
3. **Given** the calculated meat type does not support doneness selection, **When** roasting instructions are displayed, **Then** the input summary omits doneness and only shows the relevant inputs for that meat type.
4. **Given** no calculation has been run yet, **When** the user views the results area, **Then** no input summary or instruction output is shown.

---

### User Story 2 - Summary Stays Stable While Editing the Form (Priority: P1)

As a home cook experimenting with different options, I want the displayed input summary and instructions to remain tied to my last calculation until I calculate again, so that I can compare or adjust form values without losing track of what the current result represents.

**Why this priority**: This is the core behaviour that prevents confusion when form fields and results diverge. It is inseparable from Story 1 for delivering the stated user value.

**Independent Test**: Can be tested by running a calculation, changing meat type and/or weight in the form without pressing Calculate again, and confirming the summary and instructions remain unchanged.

**Acceptance Scenarios**:

1. **Given** a calculation result is already displayed, **When** the user changes meat type, weight, or doneness in the form without requesting a new calculation, **Then** the input summary and instruction details remain unchanged and continue to reflect the previous calculation.
2. **Given** a calculation result is displayed and the user has changed form values, **When** the user requests a new calculation, **Then** the input summary and instruction details update together to reflect the newly submitted values.

---

### User Story 3 - Read the Summary Quickly on Any Device (Priority: P2)

As a home cook using the app in the kitchen, I want the input summary to be easy to scan at a glance, so that I can confirm the correct joint before following timing and temperature steps.

**Why this priority**: The summary must be useful in practice, not just present. Scannability supports the primary calculate-and-follow workflow on phones and desktops.

**Independent Test**: Can be tested by completing a calculation on mobile and desktop viewports and verifying the summary is visible, readable, and positioned directly above the instruction steps without horizontal scrolling.

**Acceptance Scenarios**:

1. **Given** a calculation result is displayed, **When** the user views the results on a mobile viewport, **Then** the input summary appears above the instructions and remains fully readable without horizontal scrolling.
2. **Given** a calculation result is displayed, **When** the user views the results on a desktop viewport, **Then** the input summary uses the same terminology as the input form (meat type, weight in kilograms, doneness where applicable).

---

### Edge Cases

- What happens when the user recalculates after a validation error? The summary and instructions update only after a successful calculation; failed attempts do not replace the previous successful summary.
- How is weight shown for decimal values (e.g., 1.5 kg)? The summary displays the weight value used for the calculation with a clear kilogram unit.
- What happens when the user clears or invalidates form fields after a calculation? The previously displayed summary and instructions remain until a new successful calculation replaces them.
- How does the summary behave when only one input changes before recalculate? Only the results area summary is authoritative; form field values may differ without affecting the displayed summary.
- What happens when a new calculation returns an error? The last successful input summary and instructions remain visible if a prior successful calculation exists; otherwise no summary is shown.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST display an input summary directly above roasting instruction details whenever a successful calculation result is shown.
- **FR-002**: The input summary MUST include the meat type display name and weight in kilograms that were used for the displayed calculation.
- **FR-003**: The input summary MUST include doneness when—and only when—the calculated meat type supports doneness selection.
- **FR-004**: The input summary MUST reflect the values submitted for the most recent successful calculation, not the current live state of the input form.
- **FR-005**: The input summary MUST update when the user performs a new successful calculation and MUST remain unchanged when the user edits form fields without recalculating.
- **FR-006**: The input summary MUST use the same user-facing terminology as the input form labels (e.g., "Meat type", "Weight (kg)", "Doneness").
- **FR-007**: System MUST NOT display an input summary when no successful calculation result is available.
- **FR-008**: The input summary MUST remain visible and readable within the results area across supported mobile and desktop viewports.

### Key Entities

- **Calculation Input Summary**: A read-only snapshot of the user selections (meat type, weight, optional doneness) tied to a specific successful calculation result.
- **Roasting Instructions**: The step-by-step cooking guidance (temperatures, durations, resting time) produced from a calculation; always displayed below its corresponding input summary.
- **Active Calculation**: The most recent successful calculation whose inputs and instructions are currently shown in the results area.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: In usability testing or structured review, 100% of test participants can correctly identify which meat type and weight a displayed result refers to after changing form fields without recalculating.
- **SC-002**: Users can locate the input summary above instruction steps within 3 seconds of viewing a calculation result on both mobile and desktop layouts.
- **SC-003**: After a new successful calculation, the input summary and instructions always change together with no observed mismatch between summary values and instruction content.
- **SC-004**: Support or feedback related to "wrong instructions for my selection" or "stale results" decreases after release compared to the prior behaviour (qualitative follow-up during early use).

## Assumptions

- The improvement applies to the roasting instruction results area; the optional "Serve at" planner continues to operate against the same last successful calculation inputs already captured by the application.
- Meat type is shown using the same friendly display name the user selected in the form (e.g., "Beef", not an internal identifier).
- Weight is shown in kilograms with one decimal place when applicable, matching how users enter weight today.
- Doneness labels in the summary match user-facing form labels (e.g., "Medium", "Well Done").
- No new user actions are required; the summary appears automatically as part of displaying results.
- Clearing results explicitly (if added in future) is out of scope; this feature focuses on disambiguating displayed results while the form remains editable.
