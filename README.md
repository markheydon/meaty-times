# MeatyTimes

[![CI](https://github.com/markheydon/meaty-times/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/markheydon/meaty-times/actions/workflows/ci.yml)
[![.NET](https://img.shields.io/badge/.NET-10.0.301-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

MeatyTimes is a simple cooking assistant for roasting joints of meat. It turns meat type, weight, and doneness into clear roasting instructions, and it can also calculate a backwards serve-at schedule so a roast fits around a meal.

## Project Name and Description

MeatyTimes is a lightweight web app for home cooks who want reliable roasting guidance without searching cookbooks or the web. The current scope is centred on the roast calculator experience described in [specs/001-roast-calculator/spec.md](specs/001-roast-calculator/spec.md): support for beef, lamb, pork, chicken, and gammon; weight-based calculations; doneness handling where appropriate; and a responsive experience for mobile, tablet, and desktop.

## Technology stack

MeatyTimes is built with a modern .NET web stack:

- .NET SDK 10.0.301 via [global.json](global.json)
- C# and ASP.NET Core minimal APIs
- Blazor Server with MudBlazor
- .NET Aspire for local orchestration and service discovery
- xUnit v3 for automated testing, with Aspire hosting testing support
- Central package management through [Directory.Packages.props](Directory.Packages.props)

Key package versions currently in use include:

- Aspire Hosting Testing: 13.4.6
- MudBlazor: 9.6.0
- xUnit v3: 3.2.2
- ASP.NET Core OpenAPI: 10.0.9
- OpenTelemetry packages: 1.15.1–1.16.0

## Project architecture

The repository is organised as a distributed application:

- [src/MeatyTimes.AppHost](src/MeatyTimes.AppHost) — Aspire orchestration for the UI, API, and shared defaults
- [src/MeatyTimes.ApiService](src/MeatyTimes.ApiService) — minimal API endpoints for roast calculation and scheduling
- [src/MeatyTimes.Core](src/MeatyTimes.Core) — domain models, cooking rules, and the calculation engine
- [src/MeatyTimes.Web](src/MeatyTimes.Web) — MudBlazor-based Blazor UI
- [src/MeatyTimes.ServiceDefaults](src/MeatyTimes.ServiceDefaults) — OpenTelemetry and health-check defaults

The design and contracts for this architecture live under [specs/001-roast-calculator](specs/001-roast-calculator), including the implementation plan, data model, and API/UI contracts.

## Getting started

### Prerequisites

- .NET SDK 10.0.301 or newer (matching [global.json](global.json))
- The Aspire CLI is recommended for the full local experience

### Run locally

```powershell
aspire run
# or
dotnet run --project src/MeatyTimes.AppHost
```

Then open the webfrontend endpoint shown in the Aspire dashboard.

## Project Structure

- [src](src) — application projects for the AppHost, API, web UI, core logic, and shared defaults
- [tests](tests) — [MeatyTimes.Core.Tests](tests/MeatyTimes.Core.Tests) (domain unit tests), [MeatyTimes.Web.Tests](tests/MeatyTimes.Web.Tests) (Blazor component tests), and [MeatyTimes.AppHost.Tests](tests/MeatyTimes.AppHost.Tests) (Aspire integration tests)
- [specs](specs) — feature specifications, implementation plans, and quickstarts
- [docs](docs) — end-user documentation and guidance for how the project is presented
- [docs-internal](docs-internal) — contributor and developer documentation for internal workflows
- [.specify](.specify) — project constitution and Spec Kit configuration
- [.github](.github) — CI workflow, prompts, and repository automation

## Key features

- Roast instruction calculation for beef, lamb, pork, chicken, and gammon
- Doneness selection where applicable, with food-safe defaults for poultry and pork
- Serve-at backwards scheduling for planned meal timing
- Responsive UI built with MudBlazor for mobile, tablet, and desktop

## Development workflow

Feature work in this repository follows the Spec Kit workflow described in [specs/001-roast-calculator/tasks.md](specs/001-roast-calculator/tasks.md) and the project constitution in [.specify/memory/constitution.md](.specify/memory/constitution.md):

- Use the Spec Kit commands /speckit-specify, /speckit-plan, /speckit-tasks, and /speckit-implement for feature work
- Keep feature branches in the form NNN-short-name
- Follow the constitution's quality gates for testing, UX, security, cooking accuracy, and simplicity

## Coding standards

The constitution in [.specify/memory/constitution.md](.specify/memory/constitution.md) sets the baseline for the project:

- Domain logic belongs in [src/MeatyTimes.Core](src/MeatyTimes.Core)
- Cooking-critical rules should be clearly commented and easy to audit
- Calculation changes require unit tests before merge
- Deterministic rules should remain documented and traceable through [src/MeatyTimes.Core/Rules/cooking-rules.json](src/MeatyTimes.Core/Rules/cooking-rules.json)

## Testing

MeatyTimes uses a three-layer test strategy aligned with [.specify/memory/constitution.md](.specify/memory/constitution.md) Principle II:

| Layer | Project | Purpose |
|-------|---------|---------|
| Unit | [tests/MeatyTimes.Core.Tests](tests/MeatyTimes.Core.Tests) | Domain logic and cooking calculations |
| Component | [tests/MeatyTimes.Web.Tests](tests/MeatyTimes.Web.Tests) | Blazor UI outcomes via bunit |
| Integration | [tests/MeatyTimes.AppHost.Tests](tests/MeatyTimes.AppHost.Tests) | Aspire full-stack smoke tests |

**Tooling standard**: xUnit v3, built-in `Assert` methods only, NSubstitute for mocks when needed. Do not introduce FluentAssertions, AwesomeAssertions, Shouldly, Moq, NUnit, or MSTest. Playwright is reserved for end-to-end user-journey tests when explicitly required.

Run the unit tests for the calculation engine:

```powershell
dotnet test tests/MeatyTimes.Core.Tests
```

Run the full suite, including Aspire integration tests:

```powershell
dotnet test
```

Cooking-critical behaviour should be covered by outcome-named tests and verified before merge.

## Contributing

Contributions are welcome. Please read [CONTRIBUTING.md](CONTRIBUTING.md) and [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md) before opening a pull request.

For cooking-rule changes or new meat types, include:

- documented sources or rationale for the rule change
- unit tests in [tests/MeatyTimes.Core.Tests](tests/MeatyTimes.Core.Tests)
- user-facing examples or validation coverage where appropriate

Pull requests should also pass the CI workflow in [.github/workflows/ci.yml](.github/workflows/ci.yml).

## License

MeatyTimes is released under the MIT License. See [LICENSE](LICENSE) for details.

Copyright © Mark Heydon.
