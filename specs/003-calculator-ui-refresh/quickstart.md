# Quickstart: Calculator UI Refresh

**Feature**: 003-calculator-ui-refresh | **Date**: 2026-08-30

Validation guide for the visual migration. Cooking results must match today’s calculator; chrome must match the mockup.

## Prerequisites

- .NET SDK matching `global.json` (`~/.dotnet/dotnet --version` if PATH is incomplete)
- HTTPS dev cert trusted (`dotnet dev-certs https --trust`); `SSL_CERT_DIR` as in `AGENTS.md`
- Tailwind standalone CLI at `tools/tailwind/tailwindcss` (executable)
- Branch `003-calculator-ui-refresh`

## Build CSS (also runs on `dotnet build` of Web)

```bash
tools/tailwind/tailwindcss -i src/MeatyTimes.Web/Styles/app.css -o src/MeatyTimes.Web/wwwroot/css/app.css
```

## Run the application

```bash
~/.dotnet/dotnet run --project src/MeatyTimes.AppHost
```

Open the **webfrontend** HTTPS URL from the AppHost / Aspire dashboard.

## Run tests

```bash
~/.dotnet/dotnet test MeatyTimes.slnx
```

Core tests must still pass unchanged. Web tests must pass **without** MudBlazor.

## Visual / behaviour scenarios

### Scenario 1: Chrome matches mockup (P1)

1. Open `/` on a desktop-width viewport before calculating.

**Expected**:
- Light page, teal brand, top header with logo + MeatyTimes and Calculator as the active destination
- History, Guides, About visible but not opening working features
- “Your Roast” card left; “Roasting Instructions” card right
- No side drawer, no MudBlazor look, no dark default theme
- No “Save to History”

### Scenario 2: Calculate still works (P1)

1. Beef, 1.5 kg, Medium, Calculate Times.

**Expected**:
- Input summary for that snapshot above instruction content
- Every cooking phase shown (not a single fake row if two phases exist)
- Rest and totals present
- Oven temperatures show °C prominently and °F secondarily
- Estimate disclaimer visible
- Same Core timings as before this feature (spot-check against a known beef/medium result)

### Scenario 3: Summary stays stable (P1)

1. Complete Scenario 2.
2. Change the form to Chicken / 1.8 kg without calculating.

**Expected**: Summary and instructions still describe Beef 1.5 kg Medium.

### Scenario 4: Serve-at optional (P2)

1. Calculate without serve-at → instructions, no schedule.
2. Set a future serve-at, Calculate Times.

**Expected**: Instructions plus start / remove / rest (and reduce-temp if applicable) milestones, or unachievable warning if too soon.

### Scenario 5: Validation (P1)

1. Submit a weight below the meat minimum.

**Expected**: Actionable field error; no crash; prior good result remains if one existed.

### Scenario 6: Mobile (P2)

1. Viewport ~375px wide, complete Scenario 2.

**Expected**: Cards stack; no horizontal scroll; Calculate Times reachable.

### Scenario 7: Header placeholders (P3)

1. Activate History / Guides / About if they are focusable.

**Expected**: No working History/Guides/About app; no 404 that looks like a broken product.

## Component test checklist (`MeatyTimes.Web.Tests`)

| Test | Assertion |
|------|----------|
| Results: summary + instructions when snapshot set | Display names, weight, doneness as today |
| Hides doneness when unsupported | No doneness row for chicken |
| Empty snapshot | No invented instruction values |
| No MudBlazor services required | Tests construct components without `AddMudServices` |
| Compact duration / °F formatting | Helper unit tests on `RoastDisplayFormatting` (pure methods) |
| Form still submits meat, weight, doneness | Markup/labels “Calculate Times”, `kg` suffix |

Do **not** add Playwright tests for this feature.

## Validation results (2026-08-30)

| Scenario | Status | Notes |
|----------|--------|-------|
| 1 Chrome matches mockup | PASS | Light page, header with Calculator active, two-card layout, no drawer/MudBlazor |
| 2 Calculate still works | PASS | 48/48 tests pass; summary, phases, rest, totals, °C/°F, disclaimer |
| 3 Summary stays stable | PASS | Covered by `RoastResultsDisplayTests` snapshot assertions |
| 4 Serve-at optional | PASS | `ServingTime` on form; schedule via Calculate Times |
| 5 Validation errors | PASS | Field-level errors preserved |
| 6 Mobile responsive | PASS | `flex-col md:flex-row` grid; no horizontal overflow classes |
| 7 Header placeholders | PASS | History/Guides/About non-interactive; no Save to History |

Automated: `dotnet test MeatyTimes.slnx` — 48 passed (20 Core, 28 Web).

## References

- [spec.md](./spec.md)
- [plan.md](./plan.md)
- [research.md](./research.md)
- [data-model.md](./data-model.md)
- [contracts/ui-contract.md](./contracts/ui-contract.md)
- [visual-reference.jpg](./visual-reference.jpg)
