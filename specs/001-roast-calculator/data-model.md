# Data Model: Roast Calculator

**Feature**: 001-roast-calculator | **Date**: 2026-07-02

## Entity Relationship Overview

```text
MeatType ──< CookingRule >── CookingMethod
                │
                │ applies to
                ▼
           RoastRequest ──calculates──▶ CookingResult
                │                           │
                │ optional servingTime      │ contains phases
                ▼                           ▼
         CookingSchedule              CookingPhase[]
```

## Entities

### MeatType

Represents a supported roasting category.

| Field | Type | Description |
|-------|------|-------------|
| `id` | string (enum) | `beef`, `lamb`, `pork`, `chicken`, `gammon` |
| `displayName` | string | User-facing label (e.g., "Beef") |
| `supportsDoneness` | boolean | `true` for beef and lamb; `false` for pork, chicken, gammon |
| `minWeightKg` | decimal | Minimum supported joint weight |
| `maxWeightKg` | decimal | Maximum supported joint weight |
| `defaultCookingMethod` | CookingMethod | Always `TraditionalRoast` in V1 |

**Validation**: `id` must match a rule entry in `cooking-rules.json`.

---

### CookingMethod

Represents a roasting approach. V1 uses one value only.

| Value | V1 Status |
|-------|-----------|
| `TraditionalRoast` | Active (default) |
| `HighHeatRoast` | Reserved for future |
| `ReverseSear` | Reserved for future |

---

### Doneness

User-selected cooking level where applicable.

| Value | Applicable Meats |
|-------|----------------|
| `Rare` | Beef, lamb |
| `Medium` | Beef, lamb |
| `WellDone` | Beef, lamb (pork/chicken/gammon always well-done internally) |

**Validation**: Required when `meatType.supportsDoneness` is `true`. Must be omitted or ignored for other meats.

---

### CookingRule

Defines how a meat type is roasted under a given cooking method. Stored in `cooking-rules.json`.

| Field | Type | Description |
|-------|------|-------------|
| `meatType` | string | Matches `MeatType.id` |
| `cookingMethod` | string | `TraditionalRoast` in V1 |
| `source` | string | Documented reference for this rule set |
| `initialTemperatureC` | int | Starting oven temperature (°C) |
| `secondaryTemperatureC` | int? | Reduce-to temperature; `null` if single-temperature roast |
| `minutesPerKg` | decimal | Base cooking rate per kg |
| `initialPhaseFraction` | decimal? | Fraction of total cook time at initial temp (for two-phase roasts) |
| `donenessAdjustments` | object? | Minutes added/subtracted per doneness level (beef/lamb only) |
| `restingMinutesPerKg` | decimal | Resting duration rate |
| `minRestingMinutes` | int | Floor for resting time |
| `maxRestingMinutes` | int | Cap for resting time |
| `minCookingMinutes` | int | Floor for total cooking time |
| `maxCookingMinutes` | int | Cap for total cooking time |

**Relationships**: One rule per `(meatType, cookingMethod)` pair in V1.

---

### RoastRequest

User inputs for a calculation.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `meatType` | string | Yes | Must be a supported `MeatType.id` |
| `weightKg` | decimal | Yes | `> 0`; within meat-specific min/max; max 2 decimal places |
| `doneness` | string? | Conditional | Required for beef/lamb; ignored for others |
| `servingTime` | datetime? | No | ISO 8601 local time; must be in the future for schedule |

---

### CookingPhase

A single step in the roasting process.

| Field | Type | Description |
|-------|------|-------------|
| `order` | int | Display sequence (1-based) |
| `description` | string | User-facing step label |
| `temperatureC` | int | Oven temperature for this phase |
| `durationMinutes` | int | Duration of this phase |

---

### CookingResult

Calculated roasting instructions returned to the user.

| Field | Type | Description |
|-------|------|-------------|
| `meatType` | string | Echo of request |
| `weightKg` | decimal | Echo of request |
| `doneness` | string? | Echo of request (null if N/A) |
| `cookingMethod` | string | `TraditionalRoast` in V1 |
| `phases` | CookingPhase[] | Ordered cooking steps |
| `totalCookingMinutes` | int | Sum of phase durations |
| `restingMinutes` | int | Suggested resting time |
| `totalPreparationMinutes` | int | `totalCookingMinutes + restingMinutes` |
| `source` | string | Rule source reference (for transparency/debug) |

**Display order** (constitution Principle III):
1. Initial oven temperature and duration
2. Secondary temperature and duration (if applicable)
3. Total cooking duration
4. Resting duration
5. Total preparation time

---

### CookingSchedule

Backwards-calculated timeline from a target serving time.

| Field | Type | Description |
|-------|------|-------------|
| `servingTime` | datetime | Requested serve time |
| `startCookingTime` | datetime | When to put meat in oven |
| `temperatureChangeTime` | datetime? | When to reduce temperature; `null` if single-phase |
| `removeFromOvenTime` | datetime | End of cooking (start of resting) |
| `restingStartTime` | datetime | Same as `removeFromOvenTime` |
| `isAchievable` | boolean | `false` if serving time is too soon |
| `earliestServingTime` | datetime? | Populated when `isAchievable` is `false` |

---

## State Transitions

### Calculation Flow

```text
[Idle] ──user submits valid inputs──▶ [Calculating] ──success──▶ [Results Displayed]
                                         │
                                         └──validation error──▶ [Error Displayed] ──fix inputs──▶ [Idle]
```

### Schedule Flow

```text
[Results Displayed] ──user enters serving time──▶ [Scheduling]
                                                      │
                              ┌────────────────────────┴────────────────────────┐
                              ▼                                                 ▼
                    [Schedule Displayed]                          [Unachievable Warning]
                    (isAchievable = true)                         (shows earliestServingTime)
```

---

## Validation Rules Summary

| Rule | Error Message (example) |
|------|-------------------------|
| Weight ≤ 0 | "Enter a weight greater than 0 kg" |
| Weight below minimum | "Minimum weight for beef is 0.5 kg" |
| Weight above maximum | "Maximum weight for beef is 15 kg" |
| Doneness missing for beef/lamb | "Select a doneness level" |
| Doneness provided for chicken | Ignored silently |
| Serving time in past | "Serving time must be in the future" |
| Serving time too soon | "This serving time is too soon. Earliest possible: 14:30" |
| Unsupported meat type | "Unsupported meat type" |

---

## JSON Rule File Schema (abbreviated)

```json
{
  "version": "1.0",
  "rules": [
    {
      "meatType": "beef",
      "cookingMethod": "TraditionalRoast",
      "source": "BBC Good Food — beef roasting guide",
      "initialTemperatureC": 220,
      "secondaryTemperatureC": 160,
      "minutesPerKg": 20,
      "initialPhaseFraction": 0.25,
      "donenessAdjustments": {
        "Rare": -4,
        "Medium": 0,
        "WellDone": 8
      },
      "restingMinutesPerKg": 10,
      "minRestingMinutes": 15,
      "maxRestingMinutes": 45,
      "minCookingMinutes": 30,
      "maxCookingMinutes": 300
    }
  ]
}
```

Exact values and sources to be finalized during implementation with documented references per constitution Principle V.
