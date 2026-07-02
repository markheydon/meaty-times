# API Contract: Roast Calculator

**Base URL**: `https+http://apiservice` (Aspire service discovery) / `http://localhost:{port}` (direct)

**Content-Type**: `application/json`

**Error format**: [RFC 7807 ProblemDetails](https://datatracker.ietf.org/doc/html/rfc7807)

---

## GET /api/meats

Returns supported meat types and their configuration for populating the UI.

### Response 200

```json
{
  "meats": [
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
}
```

---

## POST /api/roast/calculate

Calculates roasting instructions from user inputs.

### Request Body

```json
{
  "meatType": "beef",
  "weightKg": 2.0,
  "doneness": "Medium"
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `meatType` | string | Yes | One of: `beef`, `lamb`, `pork`, `chicken`, `gammon` |
| `weightKg` | number | Yes | Positive decimal, max 2 decimal places |
| `doneness` | string | Conditional | `Rare`, `Medium`, or `WellDone` — required for beef/lamb |

### Response 200

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

### Response 400 (validation error)

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation failed",
  "status": 400,
  "errors": {
    "weightKg": ["Minimum weight for beef is 0.5 kg"]
  }
}
```

---

## POST /api/roast/schedule

Calculates a backwards cooking schedule from a target serving time.

### Request Body

```json
{
  "meatType": "beef",
  "weightKg": 2.0,
  "doneness": "Medium",
  "servingTime": "2026-07-05T18:00:00"
}
```

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `meatType` | string | Yes | Same as calculate |
| `weightKg` | number | Yes | Same as calculate |
| `doneness` | string | Conditional | Same as calculate |
| `servingTime` | string (ISO 8601) | Yes | Local datetime; must be in the future |

### Response 200 (achievable)

```json
{
  "servingTime": "2026-07-05T18:00:00",
  "startCookingTime": "2026-07-05T17:00:00",
  "temperatureChangeTime": "2026-07-05T17:10:00",
  "removeFromOvenTime": "2026-07-05T17:40:00",
  "restingStartTime": "2026-07-05T17:40:00",
  "isAchievable": true,
  "earliestServingTime": null,
  "instructions": {
    "totalCookingMinutes": 40,
    "restingMinutes": 20,
    "totalPreparationMinutes": 60
  }
}
```

### Response 200 (not achievable)

```json
{
  "servingTime": "2026-07-05T17:15:00",
  "startCookingTime": null,
  "temperatureChangeTime": null,
  "removeFromOvenTime": null,
  "restingStartTime": null,
  "isAchievable": false,
  "earliestServingTime": "2026-07-05T18:00:00",
  "instructions": {
    "totalCookingMinutes": 40,
    "restingMinutes": 20,
    "totalPreparationMinutes": 60
  }
}
```

### Response 400

Same ProblemDetails format as `/api/roast/calculate`.

---

## Health Endpoints (existing)

| Endpoint | Purpose |
|----------|---------|
| `GET /health` | Health check (Aspire) |
| `GET /alive` | Liveness probe |

---

## OpenAPI

OpenAPI document available at `/openapi/v1.json` in Development environment (existing Aspire pattern).
