# Testing standards

Contributor testing conventions for MeatyTimes. These are **not** Spec Kit
constitution principles: they may change if the test stack changes. Binding
quality rules (must have tests, red-green for cooking-critical work, outcome-named
assertions) live in [`.specify/memory/constitution.md`](../.specify/memory/constitution.md)
Principle III.

## Tooling standard

All automated tests MUST use:

- **xUnit v3** as the test framework
- **Built-in xUnit `Assert` methods** only
- **NSubstitute** for mocks, stubs, and test doubles when isolation is required
  (pin the version in `Directory.Packages.props`; reference it per-project only
  when needed)
- **bunit** for Blazor component tests (`MeatyTimes.Web.Tests`)
- **Playwright** for end-to-end user journeys when explicitly required (not
  currently in use)

Do **not** introduce: FluentAssertions, AwesomeAssertions, Shouldly, Moq, NUnit,
or MSTest.

Keep test dependencies to a minimum.

## Test layering

| Layer | Project | Purpose |
|-------|---------|---------|
| Unit | `tests/MeatyTimes.Core.Tests` | Domain logic and cooking calculations |
| Component | `tests/MeatyTimes.Web.Tests` | Blazor UI outcomes via bunit |
| End-to-end | Playwright (when required) | Complete user journeys — not a replacement for unit or component tests |

Playwright tests MUST focus on key user journeys and business-critical workflows.
Prefer a small number of high-value tests over brittle UI coverage.

## Running tests

```powershell
dotnet test MeatyTimes.slnx
# or
dotnet test tests/MeatyTimes.Core.Tests
dotnet test
```

Cooking-critical behaviour changes require unit tests with outcome-named
assertions before merge (constitution Principle III).
