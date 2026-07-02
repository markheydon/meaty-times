# Quickstart: Roast Calculator

**Feature**: 001-roast-calculator | **Date**: 2026-07-02

Validation guide for proving the roast calculator works end-to-end. Implementation details belong in `tasks.md`.

## Prerequisites

- .NET SDK 10.0.301+ (`dotnet --version`)
- Aspire CLI (`aspire --version`) or `dotnet run` on AppHost
- Repository on branch `001-roast-calculator`

## Run the Application

```powershell
# From repository root
aspire run
# Or:
dotnet run --project src/MeatyTimes.AppHost
```

Open the Aspire dashboard and navigate to the **webfrontend** endpoint (typically `https://localhost:7xxx`).

## Run Tests

```powershell
# All tests
dotnet test

# Calculation unit tests only (after MeatyTimes.Core.Tests is created)
dotnet test tests/MeatyTimes.Core.Tests

# Integration tests
dotnet test tests/MeatyTimes.Tests
```

## Validation Scenarios

### Scenario 1: Calculate Beef Instructions (P1 — core flow)

**Goal**: Verify primary calculate flow returns complete instructions.

1. Open `/` in the web frontend.
2. Select **Beef**, enter weight **2.0** kg, select **Medium** doneness.
3. Click **Calculate**.

**Expected**:
- Results appear within 3 seconds.
- Instructions include initial temperature, phased durations (if two-phase), total cooking time, resting time, and total preparation time.
- Steps appear in fixed order per [ui-contract.md](./contracts/ui-contract.md).

**API equivalent**:

```powershell
curl -X POST http://localhost:{api-port}/api/roast/calculate `
  -H "Content-Type: application/json" `
  -d '{"meatType":"beef","weightKg":2.0,"doneness":"Medium"}'
```

Returns 200 with `phases`, `totalCookingMinutes`, `restingMinutes`, `totalPreparationMinutes`.

---

### Scenario 2: Chicken (No Doneness) (P1 — edge case)

**Goal**: Verify doneness is not required for poultry.

1. Select **Chicken**, enter weight **1.5** kg.
2. Confirm doneness selector is hidden or disabled.
3. Click **Calculate**.

**Expected**:
- Results display safe fully-cooked instructions.
- No doneness field in the response.

---

### Scenario 3: Weight Validation (P1 — error handling)

**Goal**: Verify actionable validation errors.

1. Select **Beef**, enter weight **0.1** kg (below minimum).
2. Click **Calculate**.

**Expected**:
- Error message stating minimum weight (no stack trace).
- User can correct weight and retry.

---

### Scenario 4: Gammon Calculation (P1 — V1 meat set)

**Goal**: Verify gammon is supported (replaces turkey from original spec).

1. Select **Gammon**, enter weight **2.5** kg.
2. Click **Calculate**.

**Expected**:
- Valid instructions returned.
- No doneness selection required.

---

### Scenario 5: Serve-At Schedule (P2)

**Goal**: Verify backwards schedule calculation.

1. Complete Scenario 1 (beef 2 kg, medium).
2. Enter a serving time at least 2 hours in the future.
3. Click **Plan**.

**Expected**:
- Schedule shows: start cooking time, temperature change time (if applicable), remove from oven time, resting start time.
- All times are logically ordered and sum to the serving time.

**API equivalent**:

```powershell
curl -X POST http://localhost:{api-port}/api/roast/schedule `
  -H "Content-Type: application/json" `
  -d '{"meatType":"beef","weightKg":2.0,"doneness":"Medium","servingTime":"2026-07-05T18:00:00"}'
```

---

### Scenario 6: Unachievable Serving Time (P2 — edge case)

**Goal**: Verify warning when serving time is too soon.

1. Complete Scenario 1.
2. Enter a serving time 15 minutes from now.
3. Click **Plan**.

**Expected**:
- Warning that serving time is not achievable.
- Earliest possible serving time displayed.

---

### Scenario 7: Mobile Viewport (P3)

**Goal**: Verify responsive layout.

1. Open browser dev tools, set viewport to 375×667.
2. Complete Scenario 1.

**Expected**:
- No horizontal scrolling.
- All inputs and results readable and operable.

---

### Scenario 8: Determinism (Principle V)

**Goal**: Verify same inputs produce same outputs.

1. Calculate beef 2.0 kg medium three times.
2. Compare results.

**Expected**:
- Identical `totalCookingMinutes`, `restingMinutes`, and phase durations each time.

---

## Unit Test Checklist (MeatyTimes.Core.Tests)

After implementation, these unit tests MUST exist and pass:

| Test | Meat | Assertion |
|------|------|-----------|
| Beef medium 2 kg | beef | Returns two phases with documented temperatures |
| Lamb rare 1.5 kg | lamb | Shorter cook than well-done |
| Pork 3 kg | pork | Single safe-cook profile, no doneness |
| Chicken 1.8 kg | chicken | No doneness adjustment |
| Gammon 2.5 kg | gammon | Valid resting and cooking times |
| Weight below minimum | beef | Throws validation error |
| Weight above maximum | lamb | Throws validation error |
| Schedule achievable | beef | `isAchievable` true with correct times |
| Schedule too soon | beef | `isAchievable` false with `earliestServingTime` |

## References

- [spec.md](./spec.md) — feature requirements
- [data-model.md](./data-model.md) — entity definitions
- [contracts/roast-api.md](./contracts/roast-api.md) — API shapes
- [contracts/ui-contract.md](./contracts/ui-contract.md) — UI behaviour
