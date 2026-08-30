# Service Contract: Roast Calculator

**Implementation**: `MeatyTimes.Web.Services.RoastService` (in-process facade over `MeatyTimes.Core`)

**Registration**: `AddMeatyTimesCore()` + `AddSingleton<RoastService>()` in `MeatyTimes.Web/Program.cs`

**Error format**: `RoastServiceException` with optional field-keyed `Errors` dictionary (mirrors former API ProblemDetails shape for UI binding)

---

## GetMeats

Returns supported meat types and their configuration for populating the UI.

### Response

```json
[
  {
    "id": "beef",
    "displayName": "Beef",
    "supportsDoneness": true,
    "donenessOptions": ["Rare", "Medium", "WellDone"],
    "minWeightKg": 0.5,
    "maxWeightKg": 15.0
  },
  {
    "id": "chicken",
    "displayName": "Chicken",
    "supportsDoneness": false,
    "donenessOptions": [],
    "minWeightKg": 0.8,
    "maxWeightKg": 8.0
  }
]
```

---

## Calculate

Calculates roasting instructions from user inputs.

### Inputs

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `meatType` | string | Yes | One of: `beef`, `lamb`, `pork`, `chicken`, `gammon` |
| `weightKg` | number | Yes | Positive decimal |
| `doneness` | string | Conditional | `Rare`, `Medium`, or `WellDone` — required for beef/lamb |

Parsed via `RoastRequest.FromInputs` in `MeatyTimes.Core`.

### Response (`CookingResultDto`)

```json
{
  "meatType": "beef",
  "weightKg": 2.0,
  "doneness": "Medium",
  "cookingMethod": "TraditionalRoast",
  "phases": [
    {
      "order": 1,
      "description": "Roast at initial temperature",
      "temperatureC": 220,
      "durationMinutes": 10
    },
    {
      "order": 2,
      "description": "Reduce temperature and continue roasting",
      "temperatureC": 160,
      "durationMinutes": 30
    }
  ],
  "totalCookingMinutes": 40,
  "restingMinutes": 20,
  "totalPreparationMinutes": 60,
  "source": "BBC Good Food — beef roasting guide"
}
```

### Validation error

Throws `RoastServiceException` with `Errors` keyed by field name:

```json
{
  "weightKg": ["Minimum weight for beef is 0.5 kg"]
}
```

---

## PlanSchedule

Calculates a backwards cooking schedule from a target serving time.

### Inputs

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `meatType` | string | Yes | Same as calculate |
| `weightKg` | number | Yes | Same as calculate |
| `doneness` | string | Conditional | Same as calculate |
| `servingTime` | `DateTimeOffset` | Yes | Must be in the future |

### Response (`ScheduleDto`) — achievable

```json
{
  "servingTime": "2026-07-05T18:00:00+01:00",
  "startCookingTime": "2026-07-05T17:00:00+01:00",
  "temperatureChangeTime": "2026-07-05T17:10:00+01:00",
  "removeFromOvenTime": "2026-07-05T17:40:00+01:00",
  "restingStartTime": "2026-07-05T17:40:00+01:00",
  "isAchievable": true,
  "earliestServingTime": null,
  "instructions": {
    "totalCookingMinutes": 40,
    "restingMinutes": 20,
    "totalPreparationMinutes": 60
  }
}
```

### Response — not achievable

```json
{
  "servingTime": "2026-07-05T17:15:00+01:00",
  "startCookingTime": null,
  "temperatureChangeTime": null,
  "removeFromOvenTime": null,
  "restingStartTime": null,
  "isAchievable": false,
  "earliestServingTime": "2026-07-05T18:00:00+01:00",
  "instructions": {
    "totalCookingMinutes": 40,
    "restingMinutes": 20,
    "totalPreparationMinutes": 60
  }
}
```

### Validation error

Throws `RoastServiceException` for invalid inputs (e.g. past `servingTime`).

---

## Health Endpoints

| Endpoint | Purpose |
|----------|---------|
| `GET /health` | Health check (Development / Aspire) |
| `GET /alive` | Liveness probe (Development / Aspire) |

---

## Architecture note

This contract replaced the former HTTP API (`MeatyTimes.ApiService`) when the app was simplified to a single Blazor container. External HTTP access is not exposed; all calls are in-process within `MeatyTimes.Web`.
