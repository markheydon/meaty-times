# Feature Specification: Calculator UI Refresh

**Feature Branch**: `003-calculator-ui-refresh`

**Created**: 2026-08-30

**Status**: Draft

**Input**: User description: "Update the UI to look like the attached mocked-up screenshot. Note this is NOT a request to implement ALL the features mentioned on the screenshot, for example, it mentions History, Guides, etc. These may be implemented in future so we are designing with that in mind BUT this request is specifically re migrating the existing UI to the new version."

**Visual reference**: [visual-reference.jpg](./visual-reference.jpg)

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Recognise the New Calculator Layout (Priority: P1)

As a home cook opening MeatyTimes, I want the calculator to match the mocked-up look (branded header, two clear cards for my roast and the instructions, calm teal-and-light palette), so that the product feels like a dedicated roasting assistant rather than a generic dark admin screen.

**Why this priority**: The request is a visual migration of the existing calculator. If the page does not read as the mockup, the feature has not delivered.

**Independent Test**: Open the calculator with no prior calculation and confirm header, “Your Roast” card, results area treatment, colours, and typography match the visual reference at a glance on a typical desktop width.

**Acceptance Scenarios**:

1. **Given** a typical desktop viewport, **When** the cook opens the calculator, **Then** they see a light page with a top header (logo and product name on the left, destinations on the right), a “Your Roast” input card, and a “Roasting Instructions” area beside it, consistent with the visual reference.
2. **Given** the header, **When** the cook looks at destinations, **Then** Calculator is clearly the active destination.
3. **Given** the visual reference, **When** the cook compares colour, card treatment, spacing, and icon-led labels, **Then** the live calculator is recognisably the same design (teal primary, light grey page background, white input card, cooler tinted results card, generous padding, rounded cards).

---

### User Story 2 - Enter Roast Details in the New Form (Priority: P1)

As a home cook, I want to enter meat type, weight, and doneness (when it applies) in the restyled “Your Roast” card and request times with one primary action, so that I can still calculate roasting instructions without learning a new workflow.

**Why this priority**: Existing calculate behaviour must survive the visual change; cooks still need to get from inputs to instructions quickly.

**Independent Test**: Complete a valid calculation using only the “Your Roast” card and confirm the same kinds of instructions appear as today (temperatures, cooking durations, rest).

**Acceptance Scenarios**:

1. **Given** the “Your Roast” card, **When** the cook views the form, **Then** they see labelled fields for meat type, weight (with a clear kilogram unit), and doneness when the selected meat supports it, plus short helper text under fields where the mockup shows it.
2. **Given** valid roast details, **When** the cook uses the primary action (“Calculate Times”), **Then** roasting instructions appear in the results card without leaving the page.
3. **Given** a meat type that does not use doneness, **When** the cook fills the form, **Then** doneness is not required and the rest of the form still matches the new visual treatment.
4. **Given** invalid or missing weight (or other existing validation cases), **When** the cook requests times, **Then** they see a clear, actionable error in the form or adjacent alert area, not a blank or crashed page.

---

### User Story 3 - Read Restyled Instructions Without Losing Existing Guidance (Priority: P1)

As a home cook who has calculated a roast, I want instructions presented in the mockup’s scannable row style (icon, short explanation, prominent value) while still seeing everything the current calculator already provides, so that the new look does not hide phases, rest, totals, or which inputs the result is for.

**Why this priority**: Visual migration must not drop cooking-critical content or the input summary that prevents stale-result confusion.

**Independent Test**: Calculate a roast that has more than one cooking phase, then confirm input summary, each phase, rest, and totals are all visible in the new results card.

**Acceptance Scenarios**:

1. **Given** a successful calculation, **When** instructions are shown, **Then** the results card uses the mockup’s “Roasting Instructions” heading, supporting line, icon-led rows, and estimate disclaimer, and prominent values use the primary teal treatment.
2. **Given** a successful calculation, **When** the cook reads the results card, **Then** a concise summary of the inputs used for that calculation still appears with the instructions (meat type, weight in kilograms, doneness when it applied), and that summary does not change if they edit the form without calculating again.
3. **Given** a roast profile with a temperature change, **When** instructions are shown, **Then** both cooking stages remain visible (not collapsed into a single temperature/time if two stages apply).
4. **Given** oven temperatures in the results, **When** the cook reads a temperature row, **Then** they see Celsius as the primary figure and a Fahrenheit equivalent as secondary text, matching the mockup’s dual-unit pattern.
5. **Given** a successful calculation, **When** the cook reads the bottom of the results card, **Then** they see a short disclaimer that times are estimates and can vary with joint shape and oven performance.

---

### User Story 4 - Optional Serve Time in the New Layout (Priority: P2)

As a home cook planning when to eat, I want the existing “serve at” capability presented inside the “Your Roast” card in the mockup’s date/time style, so that I do not depend on a separate, visually unrelated planner block.

**Why this priority**: Serve-at already exists; this story only relocates and restyles it. Core calculate still works without it.

**Independent Test**: Set a future serving time, request times, and confirm start/remove/rest milestones still appear; omit serving time and still receive roasting instructions.

**Acceptance Scenarios**:

1. **Given** the “Your Roast” card, **When** the cook views the form, **Then** they can optionally set when they want to serve, with helper text consistent with the mockup.
2. **Given** roast details and no serving time, **When** the cook requests times, **Then** roasting instructions still appear and no serving timeline is required.
3. **Given** roast details and a valid future serving time, **When** the cook requests times, **Then** they still receive roasting instructions plus the existing cooking-schedule milestones (when to start, when to change temperature if applicable, when to remove, when rest begins).
4. **Given** a serving time that is too soon, **When** the cook requests a schedule, **Then** they still see an explanation that the time is not achievable and the earliest feasible serving time, using the new visual language (not a silent failure).

---

### User Story 5 - Use the New Layout in the Kitchen (Priority: P2)

As a home cook on a phone, tablet, or desktop, I want the new two-card layout to stack and stay readable, so that I can follow the mockup on a large screen and still operate every control on a small screen.

**Why this priority**: Kitchen use is often on a phone; the mockup is desktop-first and must adapt without clipping.

**Independent Test**: Complete calculate (and optional serve-at) on a narrow viewport and a wide viewport without horizontal scrolling or unreachable primary actions.

**Acceptance Scenarios**:

1. **Given** a mobile viewport, **When** the cook uses the calculator, **Then** the header, “Your Roast” card, and instructions card stack in a usable order, remain fully readable, and do not require horizontal scrolling.
2. **Given** a desktop viewport, **When** the cook views a result, **Then** input and results sit side by side as in the visual reference, with the same field order and instruction order as on mobile.
3. **Given** a narrow header, **When** destinations cannot all fit comfortably, **Then** Calculator remains reachable and future destinations do not obscure the logo or the primary calculate workflow.

---

### User Story 6 - Trust the Page Chrome Without Fake Features (Priority: P3)

As a home cook, I want the header to look ready for later History, Guides, and About destinations, without being promised working History, Guides, About, or “Save to History” in this release.

**Why this priority**: The mockup includes those items as design-forward chrome; implementing them is explicitly out of scope.

**Independent Test**: Inspect the header and results card and confirm Calculator is the only working destination, with no save-to-history action that pretends to persist a roast.

**Acceptance Scenarios**:

1. **Given** the header, **When** the cook looks for other destinations, **Then** History, Guides, and About may be shown so the chrome matches the mockup’s structure, but they do not open working History, Guides, or About experiences in this release.
2. **Given** a calculation result, **When** the cook looks at the results card header, **Then** there is no “Save to History” (or equivalent) action that claims to store the roast.
3. **Given** the page footer area, **When** a result is shown (and, if useful, before a result), **Then** a tip strip consistent with the mockup can appear (for example, reminding the cook to use a thermometer), without contradicting the calculated doneness or implying a brand/chef endorsement.

---

### Edge Cases

- How does the layout behave before any calculation? The results area still occupies the right-hand (or stacked) slot with the “Roasting Instructions” treatment, but does not invent instruction values or an input summary.
- How are multi-phase roasts shown in a design that highlights a single oven temperature and cooking time? Each cooking phase is its own instruction row; rest remains a separate row; totals remain available so nothing cooking-critical is dropped.
- What happens when doneness does not apply? The doneness field is omitted or inactive as today; the card layout still matches the mockup.
- What happens when the cook changes form fields after a result? The input summary and instruction rows stay tied to the last successful calculation (existing behaviour), even though the form now looks like the mockup.
- What happens if a calculation fails after a previous success? The last successful summary and instructions remain; the error is visible and actionable.
- How is a serving time in the past handled? Same as today: rejected or explained; the new date/time control must not hide that feedback.
- How does the tip strip behave when doneness or meat type would make a single hardcoded internal-temperature range misleading? The tip stays generic or matches the current result; it must not show a medium beef range as if it applied to poultry or a different doneness.
- What happens to the existing side drawer navigation? The mockup’s top header replaces that chrome for the calculator; cooks must not need a leftover drawer to reach Calculator.
- How does the page behave if header destinations for unimplemented features are activated? They must not appear as broken pages that look like a failed product; they are non-working in this release.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The calculator MUST present the existing calculate-and-display workflow in a layout that matches the visual reference: branded header, “Your Roast” input card, “Roasting Instructions” results card, and optional tip strip.
- **FR-002**: The visual language MUST follow the visual reference: light page background, teal primary for brand, headings, primary action, and prominent result values; white input card; cooler tinted results card; pale tip strip; rounded cards; generous spacing; stroke-style icons beside section titles and instruction rows.
- **FR-003**: The “Your Roast” card MUST include meat type, weight with an explicit kilogram unit, doneness when applicable, optional serve-at, helper text under fields as in the visual reference, and a full-width primary action labelled “Calculate Times”.
- **FR-004**: Meat type and doneness controls MUST remain selectable from the same supported options as today; presentation MAY include small meat/doneness cues consistent with the mockup without changing available choices.
- **FR-005**: The primary action MUST still produce roasting instructions from meat type, weight, and doneness (when required) using existing calculation rules; this feature MUST NOT change cooking maths, supported meats, weight ranges, or rest rules.
- **FR-006**: After a successful calculation, the results card MUST show the existing input summary (meat type, weight in kilograms, doneness when it applied) tied to that calculation, not the live unsaved form.
- **FR-007**: Instruction content MUST remain complete relative to today’s calculator: every cooking phase (temperature and duration), resting time, and total cooking / total preparation figures the cook already receives. The mockup’s three-row pattern is the visual model, not a licence to omit phases or totals.
- **FR-008**: Oven temperatures in the results MUST show Celsius as the primary value and Fahrenheit as a secondary equivalent (display conversion of the same figure).
- **FR-009**: Durations in prominent result values SHOULD use a compact form consistent with the mockup (for example, “1 hr 15 min”) while remaining unambiguous.
- **FR-010**: The results card MUST include an estimate disclaimer consistent with the visual reference.
- **FR-011**: Serve-at MUST remain optional. When omitted, instructions still appear. When provided with a successful calculation, the existing schedule milestones MUST still be available to the cook, visually aligned with the new design.
- **FR-012**: Existing validation, loading, and error behaviours MUST remain: invalid input is explained with a next step; in-progress calculation is visible; internal errors are not dumped as technical traces.
- **FR-013**: Calculator MUST be the only working destination in this release. History, Guides, and About MUST NOT be delivered as working features. If shown in the header for visual fidelity, they MUST NOT claim to work.
- **FR-014**: The results card MUST NOT include “Save to History” or any control that implies the roast is stored.
- **FR-015**: A tip strip MAY appear, matching the mockup’s placement. When it mentions internal temperature, the guidance MUST be appropriate to the displayed result (or stay generic if no result). It MUST NOT imply endorsement by a brand, chef, or publisher.
- **FR-016**: The layout MUST remain usable on small and large viewports: no essential control or instruction clipped; no horizontal scrolling for the primary workflow; side-by-side cards on wide screens, stacked on narrow screens.
- **FR-017**: Header structure MUST leave a clear place for future History, Guides, and About destinations so those can be added later without inventing a different navigation pattern.
- **FR-018**: The previous dark, drawer-based chrome MUST NOT remain the default calculator appearance; the mockup’s light header-and-cards presentation is the default.
- **FR-019**: Accessibility of the primary workflow MUST be preserved: fields remain labelled, the primary action is identifiable, instruction values remain readable, and colour is not the only way to distinguish the active destination or errors.

### Key Entities

- **Visual reference**: The attached mockup (`visual-reference.jpg`) is the source of truth for layout, colour, typography emphasis, and chrome. Where the mockup shows unimplemented product areas, this spec’s Out of Scope and FR-013/FR-014 win.
- **Your Roast card**: The input surface for meat type, weight, doneness (when applicable), optional serve-at, and Calculate Times.
- **Roasting Instructions card**: The output surface for input summary, instruction rows, disclaimer, and (when requested) serving schedule.
- **Header destinations**: Calculator (working) plus reserved slots for History, Guides, and About (not working in this release).
- **Tip strip**: Optional non-blocking advice below the cards; must stay consistent with calculated guidance when a result exists.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: In a side-by-side review against the visual reference, reviewers agree the default calculator (header, two cards, colours, primary action, instruction-row emphasis) matches the mockup’s structure and palette without needing to be told it is “the new design”.
- **SC-002**: Cooks can complete a valid calculate flow (meat, weight, doneness when required, Calculate Times) and see instructions in under the same short, single-page path as today (no extra accounts, history, or guides required).
- **SC-003**: After a successful calculation, 100% of reviewed cases still show the correct input summary for that result after the form is changed without recalculating.
- **SC-004**: For a two-phase roast, reviewers can locate both cooking stages plus rest without missing a stage that today’s calculator would have shown.
- **SC-005**: On a mobile-width viewport, the primary workflow can be completed without horizontal scrolling, and all instruction rows remain readable.
- **SC-006**: Structured review finds no working History, Guides, About, or Save-to-History behaviour, and no control that appears to save or open those areas successfully.

## Assumptions

- This is a visual and layout migration of the **existing** calculator only. Cooking rules, meat catalogue, weight limits, doneness rules, and serve-at milestone meaning stay as they are.
- History, Guides, About, and Save to History appear on the mockup as future product surfaces. They are **not** in this feature. Header chrome may reserve space for them; it must not ship those experiences.
- Serve-at is an existing capability being relocated/restyled into “Your Roast”, not a new product. A single primary “Calculate Times” action is preferred over keeping a visually separate Plan block, as long as instructions still work without a serving time.
- Dual temperature display (Celsius primary, Fahrenheit secondary) is presentation of existing Celsius values, not a change of cooking rules or user-entered units. Weight remains kilograms only.
- The mockup’s simplified three rows (one oven temperature, one cooking time, rest) are a **visual pattern**. Real results still enumerate every phase the cook needs.
- The input summary from the prior result-summary work remains required even though the mockup does not draw it; it sits in the results card without breaking the new visual hierarchy.
- Default appearance is the mockup’s light theme. A dark theme or theme toggle is not part of this mockup and is not required for this release.
- The previous side drawer is not the target navigation pattern; a top header is.
- Helper copy on the form and the estimate disclaimer can follow the mockup wording closely, adjusted only where it would be inaccurate for a given meat or doneness.
- Fahrenheit conversion is the conventional display equivalent of the shown Celsius oven setting, rounded to a cook-friendly whole number.

## Out of Scope

- Implementing History (list, persistence, or accounts).
- Implementing Guides or recipe/education content.
- Implementing About beyond any non-working header slot.
- Save to History or any stored roast list.
- Changing cooking calculations, adding meats, or changing food-safety minima.
- Meal planning, shopping lists, or user accounts.
- Replacing or reducing the input summary, phase list, rest, or totals the cook already receives.
- Making History/Guides/About clickable working pages “as a preview”.
