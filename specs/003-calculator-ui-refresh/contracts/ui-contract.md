# UI Contract: Calculator Visual Refresh

**Feature**: 003-calculator-ui-refresh | **Date**: 2026-08-30  
**Supersedes (presentation only)**: [001 ui-contract](../../001-roast-calculator/contracts/ui-contract.md) MudBlazor component table; [002 ui-contract](../../002-result-input-summary/contracts/ui-contract.md) layout of the results column.  
**Does not change**: `RoastService` method signatures, Core cooking rules, or 002 summary **semantics**.

**Route**: `/`  
**Render mode**: Interactive Server (Blazor)  
**Styling**: Tailwind v4 (standalone CLI). **Icons**: Lucide (inlined). **No** MudBlazor or other component libraries.

**Visual source**: [visual-reference.jpg](../visual-reference.jpg)

---

## Page chrome

```text
┌──────────────────────────────────────────────────────────────────┐
│  [utensils] MeatyTimes          [Calculator] History  Guides  About│
├──────────────────────────────────────────────────────────────────┤
│  ┌─ Your Roast ─────────────┐  ┌─ Roasting Instructions ─────────┐ │
│  │ Meat type                 │  │ (empty copy or summary+rows)  │ │
│  │ Weight            [ kg ] │  │ Disclaimer when result shown   │ │
│  │ Doneness (if applicable) │  │ Schedule when serve-at used   │ │
│  │ Serve at (optional)       │  └────────────────────────────────┘ │
│  │ [ Calculate Times ]      │                                     │
│  └───────────────────────────┘                                     │
│  ┌─ Tip ─────────────────────────────────────────────────────────┐ │
│  │ thermometer reminder (generic; no unsourced °C ranges)       │ │
│  └────────────────────────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────────────────┘
```

Wide viewports: two columns (form ~2/5, results ~3/5). Narrow: stack form then results. No horizontal scroll on the primary workflow.

---

## Header

| Element | Behaviour |
|---------|-----------|
| Logo + MeatyTimes | Brand; not a second product name in the page body as a competing h1 |
| Calculator | Active destination; current page |
| History, Guides, About | Visible for layout fidelity; **not** working navigation; must not 404 |
| Drawer | Removed from default calculator chrome |

---

## Your Roast (`RoastInputForm`)

| Element | Control | Behaviour |
|---------|---------|-----------|
| Meat type | Native `<select>` | Options from `GetMeats()`; doneness visibility follows selection |
| Weight | Native number + `kg` suffix | Helper: raw weight of the roast; min/max from meat |
| Doneness | Native `<select>` | Hidden when meat does not support doneness |
| Serve at | Native `datetime-local` (optional) | Helper: when the cook wants to serve; not required to calculate |
| Calculate Times | Primary full-width button | Disabled while loading; invokes parent calculate (and schedule if time set) |

Helper text under fields should follow the mockup where accurate.

Errors: existing weight/doneness/general messages, visible next to the field or in an alert region. No stack traces.

---

## Roasting Instructions (`RoastResultsDisplay`)

| Element | Behaviour |
|---------|-----------|
| Card | Always present (tinted background vs white form card) |
| Heading | “Roasting Instructions” + supporting line |
| Input summary | After success: meat type, weight, doneness if applicable; **snapshot**, not live form |
| Phase rows | One row per phase, icon + short explanation + prominent C (F secondary) and duration |
| Rest row | Prominent rest duration |
| Totals | Total cooking and total preparation remain visible |
| Disclaimer | After success: times are estimates; shape and oven vary |
| Save to History | **Absent** |

Empty (no snapshot): heading/supporting text only — no invented temperatures or times.

---

## Schedule (existing `ServeAtPlanner` restyled / folded into results)

Shown only when the last successful action included a serving time (or an equivalent successful schedule request).

| State | UI |
|-------|----------|
| Achievable | Milestones: start cooking, reduce temperature if applicable, remove from oven, rest begins — local times |
| Unachievable | Warning + earliest feasible serving time |
| Error | Actionable message; do not clear a prior good instruction snapshot |

---

## Tip strip

Always allowed below the cards. Copy must stay generic or result-accurate. **Forbidden**: presenting a single medium-beef internal range as universal.

---

## Terminology (unchanged)

| Term | Usage |
|------|--------|
| Doneness | Form and summary |
| Rest / Rest time | After oven |
| Calculate Times | Primary action label (was “Calculate”) |
| Your Roast | Input card title (was “Roast details”) |
| Roasting instructions | Results heading |

Weight remains kilograms. Oven display is Celsius primary, Fahrenheit secondary.

---

## Accessibility

- Every input has a visible label (`for`/`id` or wrapping `<label>`).
- Primary action is a real button with accessible name “Calculate Times”.
- Active header destination is not colour-only (e.g. filled control vs text).
- Instruction values remain in the accessibility tree as text, not images.
- Decorative Lucide icons are `aria-hidden="true"`.
- Error text is associated with the invalid field where practical.

---

## Out of contract (this release)

- History list, Guides content, About page, Save to History
- Dark theme, theme toggle
- Chart widgets
- Changing `RoastService` / Core contracts
