# Research: Roast Calculator

**Feature**: 001-roast-calculator | **Date**: 2026-07-02

## 1. V1 Meat Types and Doneness

**Decision**: Support beef, lamb, pork, chicken, and gammon. Doneness options: Rare, Medium, Well Done for beef and lamb only. Pork, chicken, and gammon use safe fully-cooked rules with no doneness selection.

**Rationale**: Planning input explicitly lists gammon instead of turkey. Gammon is a common UK roast joint. Poultry and cured pork require food-safe minimum internal temperatures; offering doneness choice would conflict with constitution Principle V.

**Alternatives considered**:
- *Keep turkey from spec*: Rejected — user planning scope replaces turkey with gammon.
- *Offer medium-rare for beef*: Rejected — planning scope limits to Rare / Medium / Well Done.
- *Doneness for pork*: Rejected — food-safety guidance requires well-cooked pork; misleading to offer rare/medium.

## 2. Cooking Rules Storage

**Decision**: Store cooking rules in a versioned JSON file (`cooking-rules.json`) bundled with `MeatyTimes.Core`, loaded at application startup.

**Rationale**: V1 has ~5 meat types with static rules. A JSON file is inspectable, version-controlled, diffable in PRs, and requires no cloud infrastructure or secrets. Aligns with constitution Principle VI (simplest viable design) and Principle V (traceable, documented rules with `source` field per entry).

**Alternatives considered**:
- *Azure Blob/Table Storage*: Rejected for V1 — adds Aspire resource wiring, connection strings, and network dependency for data that changes only via deployment. Suitable for a future admin UI to update rules without redeploy.
- *Hard-coded C# constants*: Rejected — rules harder to review and update; JSON keeps data separate from algorithm.
- *Database (SQL/NoSQL)*: Rejected — no relational data, no user data, over-engineered for read-only rule sets.

## 3. Cooking Method Selection

**Decision**: V1 uses Traditional Roast only. `CookingMethod` exists in the domain model as an internal property (default: `TraditionalRoast`) but is not exposed in the UI or API request body.

**Rationale**: User domain model mentions Traditional Roast, High Heat Roast, and Reverse Sear, but V1 scope inputs list only meat type, weight, and doneness. Shipping one method keeps calculation engine and tests focused.

**Alternatives considered**:
- *Expose method selector in V1*: Rejected — expands scope, requires 3× rule definitions, and delays core value delivery.
- *Omit CookingMethod from model*: Rejected — would require breaking schema change when V2 adds methods.

## 4. Architecture: Shared Core Library

**Decision**: Create `MeatyTimes.Core` class library containing domain types, `RoastCalculator`, `ScheduleCalculator`, and `CookingRuleLoader`. Referenced by `ApiService` and `MeatyTimes.Core.Tests`.

**Rationale**: Constitution Principle II requires unit tests on calculation logic without HTTP. Principle I requires domain logic in dedicated modules. A single shared library avoids duplicating rules between API and tests.

**Alternatives considered**:
- *Calculation in ApiService only*: Rejected — forces integration tests for all calculation coverage.
- *Calculation in Web with API as passthrough*: Rejected — business logic in UI layer violates separation; harder to test.
- *Interface + strategy per meat type*: Rejected — only one algorithm path in V1; violates Principle VI.

## 5. API Design Pattern

**Decision**: ASP.NET Core minimal APIs in `MeatyTimes.ApiService` with route groups: `GET /api/meats`, `POST /api/roast/calculate`, `POST /api/roast/schedule`. Return JSON; use `ProblemDetails` for validation errors.

**Rationale**: Matches existing Aspire starter pattern (minimal APIs, OpenAPI in development). Thin endpoints delegate to `MeatyTimes.Core`. Web uses typed `HttpClient` with Aspire service discovery (`https+http://apiservice`).

**Alternatives considered**:
- *Calculation only in Blazor (no API)*: Rejected — prevents future mobile/PWA clients; couples UI to domain.
- *gRPC*: Rejected — unnecessary complexity for a simple request/response calculator.

## 6. UI Framework and Render Mode

**Decision**: Blazor Server interactive components with MudBlazor on a single `/` roast calculator page. Replace starter template home page.

**Rationale**: MudBlazor already installed in `MeatyTimes.Web`. Interactive Server suits a form-heavy calculator with instant validation feedback. Responsive MudBlazor grid/layout supports spec P3 (mobile/tablet/desktop).

**Alternatives considered**:
- *Blazor WebAssembly*: Rejected — larger payload, no benefit for a lightweight calculator.
- *Static SSR only*: Rejected — interactive form controls (select, numeric input, calculate button) require interactivity.

## 7. Cooking Rule Sources

**Decision**: Initial rules derived from widely used UK home-cooking references. Each JSON rule entry includes a `source` field citing the reference (e.g., BBC Good Food, NHS food safety, established cookbook methodology). Food-safety minimums for chicken and pork follow NHS/Food Standards Agency guidance.

**Rationale**: Constitution Principle V prohibits undocumented magic numbers. Sources are recorded at rule-definition time, not shown to end users unless a future "why these times?" feature is added.

**Alternatives considered**:
- *Single generic rule for all meats*: Rejected — inaccurate; defeats product purpose.
- *Average conflicting sources without documentation*: Rejected — violates Principle V.

## 8. Serve-At Scheduling

**Decision**: Include serve-at scheduling in V1 (Spec P2). `ScheduleCalculator` works backwards from target serving time subtracting resting duration and cooking phases.

**Rationale**: Spec lists this as P2 primary use case. Low incremental cost once `CookingResult` exists. Not in user's V1 output list but not in out-of-scope list either.

**Alternatives considered**:
- *Defer to V2*: Rejected — spec acceptance scenarios already defined; implementation is a thin layer over existing durations.

## 9. Testing Strategy

**Decision**: `MeatyTimes.Core.Tests` (xUnit v3) for calculation unit tests; extend `MeatyTimes.Tests` with API integration test for `/api/roast/calculate`. Red-green-refactor for each meat type.

**Rationale**: Constitution Principle II (non-negotiable). Unit tests name business outcomes. Integration test confirms API wiring and JSON serialization.

**Alternatives considered**:
- *UI tests only (Playwright)*: Rejected as sole coverage — too slow and brittle for calculation edge cases.
- *Mocks of calculation engine in API tests*: Rejected — constitution discourages interfaces solely for mocking.
