# Data Model: Calculator UI Refresh

**Feature**: 003-calculator-ui-refresh | **Date**: 2026-08-30

## Overview

No persisted entities, no Core domain changes, no new cooking fields. This feature adds **presentation** models and a small optional field on the existing input snapshot so serve-at can live in the same form as calculate.

```text
RoastInputModel ──Calculate Times──▶ RoastService.Calculate ──▶ CookingResultDto
        │ (optional ServingTime)
        └── when set ──────────────▶ RoastService.PlanSchedule ──▶ ScheduleDto

ActiveCalculationSnapshot (_lastInput + _result [+ _schedule])
        ├── CalculationInputSummary (existing 002 rules)
        ├── Instruction rows (phases + rest + totals)
        └── Schedule milestones (when schedule requested)
```

## Entities

### RoastInputModel (existing, extended)

Defined on `RoastInputForm`. Submitted values for Calculate Times.

| Field | Type | Description |
|-------|------|-------------|
| `MeatType` | string | Internal meat ID |
| `WeightKg` | decimal | Weight in kilograms |
| `Doneness` | string? | Required in the request only when the meat supports doneness |
| `ServingTime` | `DateTimeOffset?` | **New, optional.** When set, schedule is requested in the same action |

**Validation**: Meat/weight/doneness unchanged (still enforced by Core via `RoastService`). Serving time: omit or a future local time; past / too-soon handled by existing schedule rules and UI errors.

---

### MeatTypeDto / CookingResultDto / PhaseDto / ScheduleDto (existing — unchanged)

`RoastService` DTO shapes stay as they are. UI maps them into rows; it does not recompute minutes or temperatures.

---

### CalculationInputSummary (existing 002 — retained)

Same derivation: last successful `RoastInputModel` + catalog `DisplayName` / `SupportsDoneness`. Serving time is **not** part of the summary unless later specified; this refresh does not require it.

| Field | Display rule |
|-------|-------------|
| Meat type | Always, friendly name |
| Weight | Always, `{0.0} kg` |
| Doneness | Only when `SupportsDoneness` |

---

### TemperatureDisplay (presentation — new)

| Field | Rule |
|-------|--------|
| `Celsius` | Integer °C from `PhaseDto.TemperatureC` |
| `Fahrenheit` | `round((C * 9/5 + 32) / 5) * 5` (nearest 5 °F) |

Used only in instruction temperature rows. Not stored.

---

### DurationDisplay (presentation — new)

| Use | Format |
|------|--------|
| Prominent row values | Compact: `15 min`; `1 hr`; `1 hr 15 min` |
| Totals | Same compact form, still labelled Total cooking / Rest / Total preparation |

---

### InstructionRow (presentation — new)

One visual row in the results card.

| Kind | When | Primary value |
|------|------|----------------|
| Cooking phase | Each `PhaseDto` in `Order` | Temperature (C + F) and duration |
| Rest | Always after successful calculate | `RestingMinutes` |
| Total cooking | Always after successful calculate | `TotalCookingMinutes` |
| Total preparation | Always after successful calculate | `TotalPreparationMinutes` |
| Schedule milestone | When `ScheduleDto` present and achievable | Local clock time |
| Unachievable | When schedule not achievable | Warning + earliest serving time |

---

### HeaderDestination (presentation — new)

| Id | Working in this release | Interaction |
|-----|-------------------------|-------------|
| Calculator | Yes | Current page |
| History | No | Visible, not a working link |
| Guides | No | Visible, not a working link |
| About | No | Visible, not a working link |

---

### TipStrip (presentation — new)

Non-blocking copy below the cards. No entity in Core. Copy must not introduce unsourced internal-temperature ranges.

---

## State Transitions

### Calculator page

```text
[Empty results chrome, form ready]
    │
    └── Calculate Times (valid) ──▶ [Summary + instruction rows]
                                          │
                    ┌─────────────────────┼──────────────────┐
                    │                     │                  │
         form edit, no recalc     successful recalc    failed recalc
                    │                     │                  │
                    ▼                     ▼                  ▼
         snapshot unchanged      snapshot replaced    prior snapshot kept;
                                                      field/page errors shown

    └── Calculate Times with ServingTime ──▶ same as above plus schedule
                                              (or unachievable warning)
```

### Visibility

| Condition | Form | Results chrome | Summary + rows | Schedule |
|-----------|------|----------------|----------------|----------|
| First load | Visible | Visible (no values) | Hidden | Hidden |
| Success, no serve at | Visible | Visible | Visible | Hidden |
| Success, serve at | Visible | Visible | Visible | Visible |
| Failed, no prior success | Visible + errors | Visible | Hidden | Hidden |
| Failed, prior success | Visible + errors | Visible | Prior snapshot | Prior schedule if any |

---

## Validation Rules

Unchanged except serve-at is collected on the form:

- Weight min/max from selected meat (existing)
- Doneness required only when supported (existing)
- Serving time optional; if present, existing `PlanSchedule` validation/unachievable path applies
- No cooking maths in components
