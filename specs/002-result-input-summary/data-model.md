# Data Model: Result Input Summary

**Feature**: 002-result-input-summary | **Date**: 2026-07-04

## Overview

This feature introduces no new persisted entities or API models. It adds a **presentation model** in the Web UI that snapshots user inputs at successful calculation time and renders them above roasting instructions.

```text
RoastInputModel (existing) ──on successful calculate──▶ ActiveCalculationSnapshot
                                                              │
                                                              ├──▶ CalculationInputSummary (display)
                                                              └──▶ CookingResultDto (existing instructions)
```

## Entities

### RoastInputModel (existing)

Defined in `RoastInputForm.razor`. Submitted values for a calculate request.

| Field | Type | Description |
|-------|------|-------------|
| `MeatType` | string | Internal meat ID (e.g., `beef`) |
| `WeightKg` | decimal | Weight in kilograms |
| `Doneness` | string? | Doneness value when applicable; `null` otherwise |

**Role in this feature**: Source of truth for the input summary after successful calculate. Stored in `RoastCalculator._lastInput`.

---

### MeatTypeDto (existing)

From `RoastApiClient`. Catalog metadata for label resolution.

| Field | Type | Relevant to summary |
|-------|------|---------------------|
| `Id` | string | Matches `RoastInputModel.MeatType` |
| `DisplayName` | string | Shown in summary (e.g., "Beef") |
| `SupportsDoneness` | boolean | Controls doneness row visibility |

---

### CalculationInputSummary (presentation — new)

Read-only view derived at render time from `RoastInputModel` + `MeatTypeDto` lookup. Not serialized or stored separately.

| Field | Type | Source | Display rule |
|-------|------|--------|--------------|
| `MeatTypeLabel` | string | `MeatTypeDto.DisplayName` | Always shown |
| `WeightLabel` | string | `RoastInputModel.WeightKg` formatted | Always shown; one decimal place + "kg" |
| `DonenessLabel` | string? | `RoastInputModel.Doneness` formatted | Shown only when `SupportsDoneness` is true |

**Formatting rules**:
- Weight: `{value:0.0} kg` (e.g., `2.0 kg`, `1.5 kg`)
- Doneness: same mapping as form (`WellDone` → "Well Done"; other values as-is)

---

### ActiveCalculationSnapshot (implicit UI state)

The pair of `_lastInput` + `_result` on `RoastCalculator.razor` after a successful calculate.

| Field | Type | Description |
|-------|------|-------------|
| `Input` | `RoastInputModel` | Last successfully calculated inputs |
| `Result` | `CookingResultDto` | Matching instruction output |

**Invariant**: Summary and instructions MUST both derive from the same snapshot. They update atomically on successful recalculate.

---

### CookingResultDto (existing — unchanged)

Instruction payload from API. Displayed below the input summary.

---

## State Transitions

### Results panel with input summary

```text
[No results]
    │
    └── successful calculate ──▶ [Results + Summary displayed]
                                      │
                    ┌─────────────────┼─────────────────┐
                    │                 │                 │
         form edit (no recalc)   successful recalc   failed recalc
                    │                 │                 │
                    ▼                 ▼                 ▼
    [Summary unchanged]    [Summary + results      [Summary unchanged
     Instructions unchanged]  updated together]      if prior success exists;
                                                      errors shown on form]
```

### Visibility rules

| Condition | Input summary | Instructions |
|-----------|---------------|--------------|
| No successful calculate yet | Hidden | Hidden |
| Successful calculate | Visible | Visible |
| Form edited, no recalculate | Visible (stale snapshot) | Visible (stale snapshot) |
| Successful recalculate | Updated | Updated |
| Failed recalculate with prior success | Prior snapshot visible | Prior snapshot visible |
| Failed recalculate with no prior success | Hidden | Hidden |

---

## Validation Rules

No new validation rules. Existing API validation applies to calculate requests. The summary never validates user input — it only displays a prior successful submission.

---

## Component Parameter Contract (Web)

`RoastResultsDisplay` gains parameters to render the summary:

| Parameter | Type | Required when `Result` set |
|-----------|------|---------------------------|
| `Result` | `CookingResultDto?` | — |
| `Input` | `RoastInputModel?` | Yes (must match last successful calculate) |
| `Meats` | `IReadOnlyList<MeatTypeDto>` | Yes (for display name and doneness visibility) |

Parent (`RoastCalculator.razor`) passes `_lastInput`, `_result`, and `_meats`.
