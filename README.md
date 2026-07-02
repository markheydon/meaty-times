# MeatyTimes

[![CI](https://github.com/markheydon/meaty-times/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/markheydon/meaty-times/actions/workflows/ci.yml)
[![.NET](https://img.shields.io/badge/.NET-10.0.301-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

MeatyTimes is a simple cooking assistant for roasting joints of meat. It helps home cooks turn meat type, weight, and doneness into clear roasting instructions, and it can also calculate a backwards serve-at schedule so cooking can be timed around a meal.

## Project overview

The current feature set is focused on the roast calculator experience described in [specs/001-roast-calculator/spec.md](specs/001-roast-calculator/spec.md): support for beef, lamb, pork, chicken, and gammon; weight-based calculations; doneness handling where appropriate; and a responsive web experience for mobile, tablet, and desktop.

## Technology stack

MeatyTimes is built with a modern .NET web stack:

- .NET SDK 10.0.301 via [global.json](global.json)
- C# and ASP.NET Core
- Blazor Server with MudBlazor
- .NET Aspire for local orchestration and service discovery
- xUnit v3 for automated testing
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
- [src/MeatyTimes.Core](src/MeatyTimes.Core) — domain models, cooking rules, and calculation engine
- [src/MeatyTimes.Web](src/MeatyTimes.Web) — MudBlazor-based Blazor UI
- [src/MeatyTimes.ServiceDefaults](src/MeatyTimes.ServiceDefaults) — OpenTelemetry and health-check defaults

The feature design and contracts for this architecture live under [specs/001-roast-calculator](specs/001-roast-calculator).

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

## Project structure

- [src](src) — application projects for the AppHost, API, web UI, core logic, and shared defaults
- [tests](tests) — unit tests in [tests/MeatyTimes.Core.Tests](tests/MeatyTimes.Core.Tests) and Aspire integration tests in [tests/MeatyTimes.Tests](tests/MeatyTimes.Tests)
- [specs](specs) — feature specifications, implementation plans, and quickstarts
- [.specify](.specify) — project constitution and Spec Kit configuration
- [.github](.github) — CI workflow, prompts, and repository automation

## Key features

- Roast instruction calculation for beef, lamb, pork, chicken, and gammon
- Doneness selection where applicable, with food-safe defaults for poultry and pork
- Serve-at backwards scheduling for planned meal timing
- Responsive UI built with MudBlazor for mobile, tablet, and desktop

## Development workflow

Feature work in this repository follows the Spec Kit workflow described in [specs/001-roast-calculator/tasks.md](specs/001-roast-calculator/tasks.md) and the project constitution in [.specify/memory/constitution.md](.specify/memory/constitution.md):

- Create or update specs with the Spec Kit commands
- Use feature branches in the form NNN-short-name
- Keep cooking-critical logic well documented and covered by tests

## Coding standards

The constitution in [.specify/memory/constitution.md](.specify/memory/constitution.md) sets the baseline for the project:

- Domain logic belongs in [src/MeatyTimes.Core](src/MeatyTimes.Core)
- Cooking-critical rules should be clearly commented and easy to audit
- Calculation changes require unit tests before merge
- Deterministic rules should remain documented and traceable

## Testing

Run the unit tests for the calculation engine:

```powershell
dotnet test tests/MeatyTimes.Core.Tests
```

Run the full suite, including Aspire integration tests:

```powershell
dotnet test
```

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
