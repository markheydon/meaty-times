# Quickstart: Result Input Summary

**Feature**: 002-result-input-summary | **Date**: 2026-07-04

Validation guide for proving the input summary appears above roasting instructions and stays stable while the form is edited. Implementation details belong in `tasks.md`.

## Prerequisites

- .NET SDK 10.0.301+ (`~/.dotnet/dotnet --version`)
- HTTPS dev cert trusted (`dotnet dev-certs https --trust`)
- Repository on branch `002-result-input-summary`

## Run the Application

```bash
dotnet run --project src/MeatyTimes.AppHost
```

Open the **webfrontend** URL from the Aspire dashboard (HTTPS; accept dev cert warning if prompted).

## Run Tests

```bash
# Full suite
~/.dotnet/dotnet test MeatyTimes.slnx

# Web component tests (after MeatyTimes.Web.Tests is added)
~/.dotnet/dotnet test tests/MeatyTimes.Web.Tests
```

## Validation Scenarios

### Scenario 1: Summary Appears After Calculate (P1)

**Goal**: Verify input summary shows above instructions.

1. Open `/`.
2. Select **Beef**, enter **2.0** kg, select **Medium**.
3. Click **Calculate**.

**Expected**:
- Above "Roasting instructions", a summary shows:
  - Meat type: Beef
  - Weight (kg): 2.0 kg
  - Doneness: Medium
- Instruction timeline appears below the summary.

---

### Scenario 2: Summary Stable When Form Changes (P1 — core fix)

**Goal**: Verify summary does not follow live form edits.

1. Complete Scenario 1.
2. Change meat type to **Chicken** and weight to **1.5** kg **without** clicking Calculate.

**Expected**:
- Form shows Chicken / 1.5 kg.
- Summary still shows Beef / 2.0 kg / Medium.
- Instructions unchanged from Scenario 1.

---

### Scenario 3: Summary Updates on Recalculate (P1)

**Goal**: Verify summary and instructions update together.

1. Continue from Scenario 2.
2. Click **Calculate**.

**Expected**:
- Summary updates to Chicken / 1.5 kg (no doneness row).
- Instructions update to chicken cooking profile.

---

### Scenario 4: No Doneness for Chicken (P1)

**Goal**: Verify conditional doneness row.

1. Select **Chicken**, **1.8** kg, Calculate.

**Expected**:
- Summary shows meat type and weight only.
- No doneness row in summary.

---

### Scenario 5: Failed Recalculate Preserves Prior Results (edge case)

**Goal**: Verify failed validation does not clear a good result.

1. Calculate **Beef** 2.0 kg **Medium** successfully.
2. Change weight to **0.1** kg (below minimum).
3. Click **Calculate**.

**Expected**:
- Weight field shows validation error.
- Summary still shows Beef / 2.0 kg / Medium.
- Previous instructions still visible.

---

### Scenario 6: Mobile Viewport (P2)

**Goal**: Verify summary readable on small screens.

1. Set viewport to 375×667.
2. Complete Scenario 1.

**Expected**:
- Summary visible above instructions.
- No horizontal scrolling.
- Labels and values readable.

---

## Component Test Checklist (MeatyTimes.Web.Tests)

After implementation, these tests SHOULD exist and pass:

| Test | Assertion |
|------|-----------|
| Renders summary when result and input provided | Meat type, weight, doneness labels present |
| Hides doneness when meat does not support it | No doneness row for chicken input |
| Hides entire component when result is null | No summary or instructions |
| Summary uses display name not ID | Shows "Beef" not "beef" |

---

## References

- [spec.md](../spec.md) — feature requirements
- [data-model.md](../data-model.md) — presentation model
- [contracts/ui-contract.md](./ui-contract.md) — UI behaviour
- [001 quickstart](../../001-roast-calculator/quickstart.md) — base app setup

---

## Validation Results (2026-07-04)

Automated validation completed via `dotnet test MeatyTimes.slnx` (23 tests passed: 16 Core, 5 Web component, 2 Aspire integration).

| Scenario | Status | Notes |
|----------|--------|-------|
| 1 — Summary after calculate | ✓ | Covered by `Renders_input_summary_with_display_name_weight_and_doneness` |
| 2 — Summary stable on form edit | ✓ | `_lastInput` snapshot + component parameter binding |
| 3 — Summary updates on recalculate | ✓ | `_lastInput` updated only on successful calculate |
| 4 — No doneness for chicken | ✓ | Covered by `Hides_doneness_row_when_meat_does_not_support_doneness` |
| 5 — Failed recalculate preserves result | ✓ | `HandleCalculate` no longer clears `_result` on error |
| 6 — Mobile viewport | ✓ | Responsive CSS in `RoastResultsDisplay.razor` and `RoastCalculator.razor` |
