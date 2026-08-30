# Research: Calculator UI Refresh

**Feature**: 003-calculator-ui-refresh | **Date**: 2026-08-30

All Technical Context unknowns are resolved below. Cooking rules and `RoastService` behaviour are unchanged.

---

## 1. Leave MudBlazor (and other component kits)

**Decision**: Remove MudBlazor from `MeatyTimes.Web` and `MeatyTimes.Web.Tests`. Rebuild the calculator with semantic HTML and Tailwind utility classes. Do not add Fluent UI, Radzen, Syncfusion, or any other UI kit.

**Rationale**: Project UI rules forbid component libraries. The mockup is a custom layout MudBlazor’s default chrome (drawer, dark theme, Material density) cannot match without fighting the kit. Native controls keep the stack small and testable in bunit via markup.

**Alternatives considered**:
- Keep MudBlazor and theme it to teal — rejected; still a forbidden kit and a poor match for the mockup header/cards.
- Mix MudBlazor for pickers only — rejected; same rule and leftover JS/CSS.

---

## 2. Tailwind CSS v4 standalone CLI

**Decision**: Integrate Tailwind **v4** via the official **standalone CLI** (no Node, npm, or PostCSS). Place the executable at `tools/tailwind/tailwindcss`. Input `src/MeatyTimes.Web/Styles/app.css`, output `src/MeatyTimes.Web/wwwroot/css/app.css`. Wire an MSBuild `BeforeBuild` target in `MeatyTimes.Web.csproj` that runs:

```text
tailwindcss -i Styles/app.css -o wwwroot/css/app.css
```

from the Web project directory. `Styles/app.css` MUST `@import "tailwindcss"` and include every Razor file with `@source` (at least `Components/**/*.razor` and `Components/**/*.razor.css`). No Vite, webpack, or npm Tailwind pipeline.

Pin the CLI to a Tailwind v4.x standalone release. CI (ubuntu-latest) and this repo’s WSL environment use the linux-x64 binary; document how to replace it for other OS if needed. Ensure the file is executable in git (`chmod +x`).

**Rationale**: Matches the project Tailwind build rules exactly. Generated CSS is produced on every Web build so CI does not need Node for styles.

**Alternatives considered**:
- `Tailwind.Extensions` / `TailwindMerge` NuGet that shells to npm — rejected (Node).
- Pre-built CDN Tailwind Play — rejected; cannot `@source` Razor, no `@theme` tokens, not the required CLI.

---

## 3. Lucide icons without Node

**Decision**: Add a small project-specific `LucideIcon` component that inlines the **closed set** of Lucide SVG paths used by the mockup (utensils/crossed cutlery, calculator, clock, book, info, thermometer, oven/flame, cloche, lightbulb, cow/beef, steak, calendar). Vendor those SVGs (Lucide ISC licence) under `src/MeatyTimes.Web/Components/Icons/`. Do not add an npm Lucide package or Chart.js.

**Rationale**: Icons must be Lucide; no Node. A full icon kit NuGet is unnecessary for ~12 glyphs. Inlined SVG is accessible (`aria-hidden` on decorative icons; labels stay on text).

**Alternatives considered**:
- Heroicons / Material Icons — rejected; constraint is Lucide.
- Chart.js icon fonts — rejected; no charts in the existing calculator.

---

## 4. Native form controls instead of Mud pickers

**Decision**: Use labelled native HTML:
- Meat type / doneness: `<select>`
- Weight: `<input type="number">` with a visible `kg` suffix
- Serve at: `<input type="datetime-local">` (optional)
- Primary action: `<button type="button">` (or submit inside a form that `preventDefault`s full page post)

Style with Tailwind. Validation messages stay next to fields / in an alert region as today.

**Rationale**: No component library. Native datetime-local covers “Today, 7:00 PM” intent without a custom calendar widget. Keyboard and mobile pickers come for free.

**Alternatives considered**:
- Separate date + time inputs matching current MudDatePicker/MudTimePicker — acceptable fallback if datetime-local proves awkward in bunit; prefer one field to match the mockup.
- Custom calendar widget — rejected as extra complexity (Principle II).

---

## 5. Serve-at: relocate, do not add or remove capability

**Decision**: Move the optional serving time into the “Your Roast” card. Extend `RoastInputModel` with optional `ServingTime`. The single **Calculate Times** action:
- always calls existing `RoastService.Calculate`
- additionally calls existing `RoastService.PlanSchedule` when a serving time is present

Schedule milestones render in the results column (restyle of today’s `ServeAtPlanner` output). No second Plan button. Omit serving time → instructions only, as today before Plan.

**Rationale**: Spec FR-011 and “no new features / no removals”. One primary action matches the mockup. Domain schedule maths stays in Core via the existing service.

**Alternatives considered**:
- Keep a separate Plan button styled like the mockup — rejected; two CTAs fight the mockup and are easy to miss.
- Drop serve-at until a later feature — rejected; that would remove an existing feature.

---

## 6. Instruction rows vs mockup’s three rows

**Decision**: Use the mockup’s **row pattern** (icon, short label, prominent value) per **cooking phase**, plus rest, plus totals. Do not collapse a two-phase roast into one oven temperature. Add Fahrenheit as **display-only** next to each Celsius oven temperature.

**Fahrenheit**: Convert with \( F = C \times 9/5 + 32 \), then round oven settings to the nearest 5 °F so 180 °C displays as 350 °F (kitchen convention, matches the mockup). This is presentation in Web (`RoastDisplayFormatting`); Core continues to emit Celsius only.

**Durations**: Compact form for prominent values (e.g. `1 hr 15 min`); keep unambiguous wording. Totals remain visible.

**Rationale**: Spec FR-007 and FR-008. Cooking-critical content cannot be dropped for visual fidelity.

**Alternatives considered**:
- Show only first-phase temperature as in the PNG — rejected; hides reduce-temperature steps.
- Put °F conversion in Core — rejected; not a cooking rule (Principle I).

---

## 7. Input summary and empty results card

**Decision**: Keep the 002 input summary in the results card (tied to `_lastInput`, not live form). The results **card chrome** (heading, supporting line) is always visible so the two-column layout matches the mockup before calculate; instruction rows, summary, and disclaimer values appear only after a successful calculation (summary/instructions still hidden when there is no snapshot).

**Rationale**: Spec empty-state edge case + FR-006.

---

## 8. Header destinations and Save to History

**Decision**: Top header replaces `MudAppBar` + `MudDrawer`. Logo + “MeatyTimes” on the left. Calculator is the current page (visually active). History, Guides, and About render as **non-interactive** labelled items (not links to empty routes) so chrome matches the mockup without fake pages. **Do not** render Save to History.

**Rationale**: Spec FR-013, FR-014, FR-017. Disabled-looking links that 404 would fail SC-006.

---

## 9. Tip strip without unsourced internal temperatures

**Decision**: Show a tip strip in the mockup’s footer placement. Because Core does **not** expose target internal temperatures, the tip MUST stay generic (e.g. use a meat thermometer; times are estimates) and MUST NOT hard-code “60–63 °C for medium” as if it applied to every meat/doneness.

**Rationale**: Constitution VII (traceability, no magic numbers). Spec FR-015.

**Alternatives considered**:
- Copy the mockup’s medium beef range verbatim — rejected; misleading for poultry/pork and unsourced.

---

## 10. Testing

**Decision**:
- Keep **xUnit v3** + built-in `Assert` + **bunit** for component tests.
- Rewrite `RoastResultsDisplayTests` (and add form/layout tests as needed) without MudBlazor test services.
- **Do not** add Playwright for this feature: it is a visual migration of an existing journey, not a new end-to-end workflow. Playwright remains the standard **if** a later feature requires a full user journey.
- Do not add FluentAssertions, Shouldly, Moq, NUnit, or MSTest.
- NSubstitute stays available in `Directory.Packages.props`; use only if a new Web test needs isolation (prefer rendering real components).
- Aspire AppHost modelling remains untested.
- `RoastServiceTests` stay; they do not depend on MudBlazor.

**Rationale**: Matches testing standards and “UI refresh only”. Adding Playwright now would be speculative coverage of an already-tested calculate path.

---

## 11. Chart.js

**Decision**: Do not add Chart.js. No existing feature requires a chart.

---

## 12. Theme and reconnect UI

**Decision**: Default is the mockup **light** palette (`#005f63` primary, light grey page, tinted results card, pale tip strip). Remove unused dark-mode toggle and Mud theme palettes. `ReconnectModal` is already HTML/CSS/JS — restyle with Tailwind if needed; keep reconnect behaviour.

**Rationale**: Spec FR-018.

---

## 13. Documentation stack migration

**Decision**: Update `docs-internal/tech-stack.md`, `README.md`, and AGENTS.md UI mentions from MudBlazor to Blazor Server + Tailwind v4 standalone CLI + Lucide. Testing standards already name bunit + optional Playwright; no constitution change.

**Rationale**: Project constraints say migrate docs if the stack is not already in place.
