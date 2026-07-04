# UI Contract: Result Input Summary

**Feature**: 002-result-input-summary | **Date**: 2026-07-04  
**Extends**: [001-roast-calculator ui-contract](../../001-roast-calculator/contracts/ui-contract.md)

**Route**: `/` (unchanged)

---

## Page Layout (updated results column)

```text
┌─────────────────────────────────────────┐
│  [Form column — unchanged]              │
│                                         │
│  ─── Results (after calculate) ───      │
│  ┌─ Calculation for ─────────────────┐  │
│  │ Meat type:    Beef                │  │
│  │ Weight (kg):  2.0 kg              │  │
│  │ Doneness:     Medium   (if N/A)   │  │
│  └───────────────────────────────────┘  │
│                                         │
│  Roasting instructions                  │
│  Step 1: Preheat oven to 220°C         │
│  Step 2: ...                            │
│  Total cooking: 40 minutes              │
│  Resting: 20 minutes                    │
│  Total preparation: 1 hour              │
│                                         │
│  ─── Serve At (optional — unchanged) ─  │
└─────────────────────────────────────────┘
```

---

## Components

### RoastResultsDisplay (updated)

| Element | Component | Behaviour |
|---------|-----------|-----------|
| Input summary block | `MudStack` + `MudText` | Rendered above "Roasting instructions" heading when `Result` and `Input` are both non-null |
| Meat type row | `MudText` | Label "Meat type:" + resolved `DisplayName` from meat catalog |
| Weight row | `MudText` | Label "Weight (kg):" + value formatted to one decimal + " kg" |
| Doneness row | `MudText` | Label "Doneness:" + friendly doneness label; **hidden** when selected meat `supportsDoneness` is false |
| Instructions | (unchanged) | `MudTimeline`, totals below summary block |

**Summary block title** (optional `MudText Typo.subtitle2`): "Calculation for" or section divider — implementation may use a subtle heading; must not compete with "Roasting instructions" hierarchy.

### RoastCalculator.razor (updated)

| Change | Behaviour |
|--------|-----------|
| Pass `Input` and `Meats` to `RoastResultsDisplay` | `_lastInput` and `_meats` forwarded alongside `_result` |
| Failed recalculate handling | When a prior successful result exists, do **not** clear `_result` or `_lastInput` on validation/API error; show field errors only |

### RoastInputForm (unchanged)

Form remains fully editable after calculate. No summary rendered in the form column.

### ServeAtPlanner (unchanged)

Continues to use `LastInput` + `Result`. No duplicate input summary required in serve-at section.

---

## Terminology (must be consistent)

| Term | Usage in summary |
|------|------------------|
| Meat type | Label prefix exactly as form label |
| Weight (kg) | Label prefix exactly as form label |
| Doneness | Label prefix exactly as form label; values match form display (e.g., "Well Done") |

---

## States (updated)

| State | UI Behaviour |
|-------|-------------|
| Initial | Form visible; results and summary hidden |
| Loading | Calculate button disabled; progress indicator; **existing summary and instructions remain visible** during recalculate if previously shown |
| Results | Input summary visible above instructions |
| Form diverged | Form values may differ from summary; summary unchanged until new successful calculate |
| Validation error (first calculate) | No summary; field errors on form |
| Validation error (recalculate) | Prior summary and instructions remain; field errors on form |
| Successful recalculate | Summary and instructions update together |

---

## Responsive Behaviour

| Viewport | Layout |
|----------|--------|
| ≥ 600px | Summary stacks above instructions in results column; labels and values readable |
| < 600px | Summary full width; no horizontal scroll; appears above instruction timeline |

---

## Accessibility

- Summary text is read-only; not focusable as input
- Label/value pairs use visible text labels (not colour alone)
- Screen readers encounter summary before instruction steps in DOM order
- Heading hierarchy: page title → form section heading → summary labels (body text) → "Roasting instructions" (h6)

---

## Out of Scope

- Editing inputs from the summary block
- Showing summary in the serve-at planner section
- API or calculation rule changes
