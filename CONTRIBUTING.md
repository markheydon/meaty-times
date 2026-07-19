# Contributing to MeatyTimes

Thanks for your interest in contributing — all help is welcome, whether that's
bug fixes, new features, documentation, tests, or cooking-rule improvements.

## Getting Started

1. Fork the repository and clone your fork.
2. Ensure you have the [.NET SDK](global.json) installed (and the [Aspire CLI](https://aspire.dev/docs/get-started/install-cli/) if you want to run the full distributed app).
3. Run the app locally:

   ```powershell
   aspire run
   # or
   dotnet run --project src/MeatyTimes.AppHost
   ```

4. Run the test suite before opening a pull request:

   ```powershell
   dotnet test
   ```

   Test projects: `MeatyTimes.Core.Tests` (domain logic), `MeatyTimes.Web.Tests` (Blazor components), `MeatyTimes.AppHost.Tests` (full-stack Aspire integration).

CI on `main` also runs `dotnet format --verify-no-changes`, so formatting should pass locally.

## Pull Requests

1. Create a branch from `main` with a short, descriptive name (for example `fix/chicken-rest-time` or `add-turkey-rules`).
2. Keep changes focused — smaller PRs are easier to review and merge.
3. Add or update tests when behaviour changes, especially for cooking calculations.
4. Open a pull request with a clear description of what changed and why.
5. Ensure CI checks pass.

You do not need to follow any particular internal workflow or tooling to
contribute. Some maintenance work in this repo uses [Spec Kit](specs/) for
feature planning, but that is optional for contributors — a well-described PR
is enough.

## Cooking Rules and Calculations

Changes to roasting times, temperatures, or meat-type behaviour should:

- Update `src/MeatyTimes.Core/Rules/cooking-rules.json` with a documented `source` for each rule
- Include unit tests in `tests/MeatyTimes.Core.Tests` that describe the expected outcome
- Respect food-safety minimums for poultry and pork (these override user doneness preferences)

If you are unsure about a rule change, open an issue to discuss before investing
in a large PR.

## Testing

All automated tests follow the tooling standard in [.specify/memory/constitution.md](.specify/memory/constitution.md) Principle II.

### Test projects

| Project | Role |
|---------|------|
| `tests/MeatyTimes.Core.Tests` | Unit tests for domain logic and cooking calculations |
| `tests/MeatyTimes.Web.Tests` | Blazor component tests (bunit) asserting user-facing outcomes |
| `tests/MeatyTimes.AppHost.Tests` | Aspire integration smoke tests (API + web endpoints) |

### Allowed stack

- **xUnit v3** — test framework for all automated tests
- **Built-in `Assert` methods** — assertions only (no FluentAssertions, Shouldly, etc.)
- **NSubstitute** — mocks and stubs when isolation is required (add to a project only when needed)
- **bunit** — Blazor component unit tests
- **Aspire.Hosting.Testing** — full-stack integration tests
- **Playwright** — end-to-end user-journey tests when explicitly required (not yet in use)

### Forbidden libraries

Do not introduce FluentAssertions, AwesomeAssertions, Shouldly, Moq, NUnit, or MSTest.

### Running tests

```powershell
dotnet test
```

Cooking-critical behaviour changes require unit tests with outcome-named assertions before merge.

## Code Style

- Domain logic belongs in `MeatyTimes.Core`
- Nullable reference types and implicit usings are enabled solution-wide
- Non-trivial cooking logic should include comments explaining the rule and intent
- Match the style of surrounding code

## Community

Please read our [Code of Conduct](CODE_OF_CONDUCT.md) before participating.

For security issues, see [SECURITY.md](SECURITY.md) — do not report vulnerabilities in public issues.

## Questions

Open a [GitHub issue](https://github.com/markheydon/meaty-times/issues) for
bugs, feature ideas, or questions. For roast-calculator behaviour, the
[feature quickstart](specs/001-roast-calculator/quickstart.md) may also be
helpful context.
