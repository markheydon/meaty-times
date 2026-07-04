# Research: Result Input Summary

**Feature**: 002-result-input-summary | **Date**: 2026-07-04

## 1. Data Source for the Input Summary

**Decision**: Bind the displayed summary to the existing `_lastInput` snapshot in `RoastCalculator.razor`, resolved to user-facing labels using the loaded meat catalog (`_meats`).

**Rationale**: The parent page already stores `RoastInputForm.RoastInputModel` on every successful calculate and passes it to `ServeAtPlanner`. Reusing this snapshot satisfies FR-004 (summary reflects last successful calculation, not live form state) without new state management. Display names come from `MeatTypeDto.DisplayName` rather than internal IDs in `CookingResultDto.MeatType`.

**Alternatives considered**:
- *Read live form field values*: Rejected — causes the exact confusion this feature fixes.
- *Use `CookingResultDto` echo fields only*: Rejected — API returns meat type ID and raw doneness enum strings (e.g., `WellDone`), not the friendly labels shown in the form.
- *New API field for display summary*: Rejected — presentation concern only; violates Principle VI.

## 2. Component Placement

**Decision**: Render the input summary inside `RoastResultsDisplay.razor`, directly above the existing "Roasting instructions" heading and timeline.

**Rationale**: Keeps all result-area content in one component. The summary and instructions always appear and update together, matching spec acceptance scenarios. `ServeAtPlanner` remains unchanged; it already uses the same `_lastInput` snapshot.

**Alternatives considered**:
- *New sibling component between form and results*: Rejected — splits result presentation across two components for no user benefit.
- *Summary inside `RoastInputForm`*: Rejected — form is editable; summary must live in the results column.

## 3. Summary Presentation Pattern

**Decision**: Show a compact labelled block using MudBlazor (`MudText` / `MudStack`) with labels matching the form: "Meat type", "Weight (kg)", and "Doneness" (conditional). Weight formatted to one decimal place with "kg" suffix. Doneness formatted with the same friendly mapping as the form (`WellDone` → "Well Done").

**Rationale**: Constitution Principle III requires consistent terminology and scannable step order. Reusing form label text and doneness formatting avoids a second vocabulary. MudBlazor matches existing results styling (`MudPaper`, typography hierarchy).

**Alternatives considered**:
- *Single-line sentence (e.g., "Beef, 2.0 kg, Medium")*: Rejected — harder to scan quickly and less consistent with labelled form fields.
- *MudChip set*: Rejected — chips imply interactivity; summary is read-only.

## 4. API and Domain Changes

**Decision**: No changes to `MeatyTimes.Core`, `MeatyTimes.ApiService`, or API contracts.

**Rationale**: `CookingResultDto` and calculation logic are unaffected. This is a UI clarity improvement on existing data already held client-side after calculate.

**Alternatives considered**:
- *Add `inputSummary` to API response*: Rejected — duplicates data the client already submitted; adds serialization and contract churn for zero domain value.

## 5. Behaviour on Failed Recalculation

**Decision**: When a new calculate attempt fails validation, keep the previous successful `_result`, `_lastInput`, and displayed summary unchanged.

**Rationale**: Matches spec edge cases and existing `HandleCalculate` behaviour (errors clear `_result` today — **implementation note**: verify current code; on error `_result = null` which would hide results). 

Wait - let me re-read HandleCalculate:

```csharp
catch (RoastApiException ex)
{
    _result = null;  // This clears results on error!
    ...
}
```

The spec says: "When a new calculation returns an error? The last successful input summary and instructions remain visible if a prior successful calculation exists"

So the implementation may need to change error handling to NOT clear `_result` and `_lastInput` on validation failure when a prior success exists. This is an important design decision for research/plan.

**Decision (revised)**: On calculate validation failure, preserve the previous successful `_result` and `_lastInput` if they exist; show field-level errors without clearing the results panel.

**Rationale**: Spec edge case explicitly requires stale successful results to remain when recalculate fails. Current code clears results on error — this must be fixed as part of the feature.

**Alternatives considered**:
- *Keep clearing results on error*: Rejected — contradicts spec edge cases and degrades UX when experimenting with invalid weights.

## 6. Testing Strategy

**Decision**: Add focused Blazor component tests using **bUnit** in a new `MeatyTimes.Web.Tests` project (or extend integration coverage via rendered markup assertion). Primary cases: summary visible after result, doneness hidden for chicken, summary unchanged when parameters change without new result, summary updates on new successful calculate.

**Rationale**: Constitution Principle II applies to cooking-critical logic; this feature is presentation-only, so unit tests on the results component are sufficient. bUnit is the standard Blazor component test approach and avoids full Aspire startup for UI markup checks. Manual quickstart scenarios cover end-to-end validation.

**Alternatives considered**:
- *Aspire integration test only*: Rejected — Blazor Server interactive markup is hard to assert without component-level tests.
- *No automated tests*: Rejected — behaviour is testable and regression-prone (especially error-handling preservation).
- *Playwright E2E*: Acceptable for quickstart manual validation; heavier than needed for primary CI gate.

## 7. Doneness Visibility Rule

**Decision**: Show doneness in the summary only when the meat type associated with `_lastInput` has `SupportsDoneness == true` in the meat catalog.

**Rationale**: Matches FR-003 and existing form behaviour. Uses catalog metadata rather than inferring from whether `Doneness` is null, in case of inconsistent payloads.

**Alternatives considered**:
- *Always show doneness when non-null*: Acceptable fallback but could show doneness for meats where the field was ignored — catalog flag is authoritative.
