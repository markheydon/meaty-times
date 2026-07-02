# UI Contract: Roast Calculator Page

**Route**: `/` (replaces starter home page)

**Render mode**: Interactive Server (Blazor)

**Framework**: MudBlazor components

---

## Page Layout

```text
┌─────────────────────────────────────────┐
│  MeatyTimes                             │
│  ─────────────────────────────────────  │
│  [Meat Type ▼]                          │
│  [Weight (kg)    ]                      │
│  [Doneness ▼]     (hidden if N/A)       │
│  [ Calculate ]                          │
│                                         │
│  ─── Results (after calculate) ───      │
│  Step 1: Preheat oven to 220°C         │
│  Step 2: Roast for 10 minutes at 220°C │
│  Step 3: Reduce to 160°C, roast 30 min │
│  Total cooking: 40 minutes              │
│  Resting: 20 minutes                    │
│  Total preparation: 1 hour              │
│                                         │
│  ─── Serve At (optional) ───            │
│  [Serving time    ] [ Plan ]            │
│  Schedule milestones (if planned)       │
└─────────────────────────────────────────┘
```

---

## Components

### RoastInputForm

| Element | Component | Behaviour |
|---------|-----------|-----------|
| Meat type | `MudSelect` | Populated from `GET /api/meats`; triggers doneness visibility |
| Weight | `MudNumericField<decimal>` | Min/max from selected meat; step 0.1; suffix "kg" |
| Doneness | `MudSelect` | Visible only when `supportsDoneness` is true; options from API |
| Calculate | `MudButton` | Calls `POST /api/roast/calculate`; shows loading state |

### RoastResultsDisplay

| Element | Component | Behaviour |
|---------|-----------|-----------|
| Instruction steps | `MudTimeline` or ordered `MudList` | One item per `CookingPhase` in order |
| Total cooking | `MudText` | Formatted as "X hours Y minutes" or "Y minutes" |
| Resting | `MudText` | Formatted duration |
| Total preparation | `MudText` (emphasised) | Cooking + resting |

### ServeAtPlanner

| Element | Component | Behaviour |
|---------|-----------|-----------|
| Serving time | `MudDatePicker` + `MudTimePicker` or `MudTextField` | Future datetime only |
| Plan | `MudButton` | Calls `POST /api/roast/schedule` |
| Schedule | `MudAlert` or `MudList` | Shows milestones with clock times |
| Unachievable warning | `MudAlert Severity.Warning` | Shows `earliestServingTime` |

---

## Terminology (must be consistent)

| Term | Usage |
|------|-------|
| Doneness | Selector label and results |
| Resting time | Duration after removing from oven |
| Initial temperature | First oven setting |
| Reduce-to temperature | Secondary oven setting (when applicable) |
| Total preparation time | Cooking + resting combined |
| Serving time | Target time to serve/eat |

---

## States

| State | UI Behaviour |
|-------|-------------|
| Initial | Form visible; results hidden; no errors |
| Loading | Calculate/Plan buttons disabled; `MudProgressLinear` indeterminate |
| Results | Instruction steps visible; serve-at section enabled |
| Validation error | `MudAlert Severity.Error` with field-level messages from API |
| Schedule warning | `MudAlert Severity.Warning` when serving time not achievable |
| Empty (no calculate yet) | Results and schedule sections hidden |

---

## Responsive Behaviour

| Viewport | Layout |
|----------|--------|
| ≥ 600px | Form and results side-by-side or stacked with comfortable padding |
| < 600px | Single column; full-width inputs; no horizontal scroll |
| ≥ 320px | All content readable and operable (spec SC-006) |

---

## Accessibility

- All form fields have `MudInput` labels (not placeholder-only)
- Error messages associated with fields via `Error` and `ErrorText` properties
- Results use semantic heading hierarchy (`MudText Typo.h5/h6`)
- Colour is not the sole indicator of state (icons/text accompany severity colours)

---

## Navigation

- `/` — Roast Calculator (primary and only feature page in V1)
- Remove starter Counter and Weather pages from nav (or hide in V1)
