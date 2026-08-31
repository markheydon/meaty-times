# MeatyTimes

[![CI](https://github.com/markheydon/meaty-times/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/markheydon/meaty-times/actions/workflows/ci.yml)
[![CD](https://github.com/markheydon/meaty-times/actions/workflows/cd.yml/badge.svg?branch=main)](https://github.com/markheydon/meaty-times/actions/workflows/cd.yml)
[![.NET](https://img.shields.io/badge/.NET-10.0.300-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

[![Live demo](https://img.shields.io/badge/Live%20demo-Azure%20Container%20Apps-0078D4?logo=microsoftazure&logoColor=white)](https://webfrontend.yellowcoast-79687552.ukwest.azurecontainerapps.io/)
[![Docs](https://img.shields.io/badge/docs-meatytimes.markheydon.me.uk-24292f?logo=githubpages&logoColor=white)](https://meatytimes.markheydon.me.uk/)
[![GitHub stars](https://img.shields.io/github/stars/markheydon/meaty-times?style=flat-square)](https://github.com/markheydon/meaty-times/stargazers)
[![GitHub issues](https://img.shields.io/github/issues/markheydon/meaty-times?style=flat-square)](https://github.com/markheydon/meaty-times/issues)
[![Last commit](https://img.shields.io/github/last-commit/markheydon/meaty-times/main?style=flat-square)](https://github.com/markheydon/meaty-times/commits/main/)

[![Blazor Server](https://img.shields.io/badge/Blazor-Server-512BD4?logo=blazor&logoColor=white)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![.NET Aspire](https://img.shields.io/badge/.NET%20Aspire-13.5-512BD4?logo=dotnet)](https://aspire.dev/)
[![Tailwind CSS](https://img.shields.io/badge/Tailwind%20CSS-v4-06B6D4?logo=tailwindcss&logoColor=white)](https://tailwindcss.com/)
[![xUnit](https://img.shields.io/badge/xUnit-v3-2E9AE2?logo=xunit&logoColor=white)](https://xunit.net/)
[![Dependabot](https://img.shields.io/badge/dependabot-enabled-025E8C?logo=dependabot&logoColor=white)](.github/dependabot.yml)

MeatyTimes is a simple cooking assistant for roasting joints of meat. It turns meat type, weight, and doneness into clear roasting instructions, and it can also calculate a backwards serve-at schedule so a roast fits around a meal.

| | |
|---|---|
| **Live app** | [webfrontend.yellowcoast-79687552.ukwest.azurecontainerapps.io](https://webfrontend.yellowcoast-79687552.ukwest.azurecontainerapps.io/) |
| **Docs** | [meatytimes.markheydon.me.uk](https://meatytimes.markheydon.me.uk/) |
| **Tests** | 58 automated tests (Core unit + Blazor component), run in CI |
| **Hosting** | Azure Container Apps via .NET Aspire (`aspire deploy`) |

## Project Name and Description

MeatyTimes is a lightweight web app for home cooks who want reliable roasting guidance without searching cookbooks or the web. The current scope is centred on the roast calculator experience described in [specs/001-roast-calculator/spec.md](specs/001-roast-calculator/spec.md): support for beef, lamb, pork, chicken, and gammon; weight-based calculations; doneness handling where appropriate; and a responsive experience for mobile, tablet, and desktop.

Later features add an input summary above results so instructions stay tied to the last calculation ([specs/002-result-input-summary](specs/002-result-input-summary)) and a refreshed calculator UI with a light teal-and-white layout ([specs/003-calculator-ui-refresh](specs/003-calculator-ui-refresh)).

## Technology Stack

MeatyTimes is built with a modern .NET web stack:

- .NET SDK 10.0.300 via [global.json](global.json) (`rollForward: latestFeature`)
- C# and ASP.NET Core (Blazor Server)
- Blazor Server with Tailwind CSS v4 (standalone CLI) and Lucide icons
- .NET Aspire for local orchestration and Azure Container Apps deployment
- xUnit v3 for automated testing
- Central package management through [Directory.Packages.props](Directory.Packages.props)

Key package versions currently in use include:

- Aspire Hosting Azure App Containers: 13.5.3
- Tailwind CSS v4 standalone CLI (no Node/npm) at `tools/tailwind/tailwindcss`
- xUnit v3: 4.0.0
- bunit: 2.9.0
- NSubstitute: 5.3.0
- OpenTelemetry packages: 1.18.0

## Project Architecture

The repository is organised as a single-container Aspire app:

- [src/MeatyTimes.AppHost](src/MeatyTimes.AppHost) — Aspire orchestration for the web UI and shared defaults
- [src/MeatyTimes.Core](src/MeatyTimes.Core) — domain models, cooking rules, and the calculation engine
- [src/MeatyTimes.Web](src/MeatyTimes.Web) — Blazor Server UI with Tailwind CSS (calls Core in-process)
- [src/MeatyTimes.ServiceDefaults](src/MeatyTimes.ServiceDefaults) — OpenTelemetry and health-check defaults

The design and contracts for this architecture live under [specs/001-roast-calculator](specs/001-roast-calculator), including the implementation plan, [data model](specs/001-roast-calculator/data-model.md), and [API/UI contracts](specs/001-roast-calculator/contracts/).

## Getting Started

### Prerequisites

- .NET SDK 10.0.300 or newer (matching [global.json](global.json))
- The [Aspire CLI](https://aspire.dev/docs/get-started/install-cli/) is recommended for the full local experience
- A trusted ASP.NET Core HTTPS dev certificate — required because the AppHost health-checks `webfrontend` over HTTPS. If startup hangs, run `dotnet dev-certs https --trust`.

### Run locally

```powershell
aspire run
# or
dotnet run --project src/MeatyTimes.AppHost
```

Then open the **webfrontend** endpoint shown in the Aspire dashboard (HTTPS; browsers may warn about the self-signed dev cert).

Tailwind CSS is built automatically on `MeatyTimes.Web` build via the standalone CLI. If the binary is missing, run `tools/tailwind/download.sh` (or `download.ps1` on Windows) or `dotnet build` on the Web project.

## Project Structure

- [src](src) — application projects for the AppHost, web UI, core logic, and shared defaults
- [tests](tests) — [MeatyTimes.Core.Tests](tests/MeatyTimes.Core.Tests) (domain unit tests) and [MeatyTimes.Web.Tests](tests/MeatyTimes.Web.Tests) (Blazor component tests)
- [specs](specs) — Spec Kit feature specifications, plans, and quickstarts
  - [001-roast-calculator](specs/001-roast-calculator) — core calculator
  - [002-result-input-summary](specs/002-result-input-summary) — input summary on results
  - [003-calculator-ui-refresh](specs/003-calculator-ui-refresh) — visual UI migration
- [docs](docs) — end-user documentation (GitHub Pages at [meatytimes.markheydon.me.uk](https://meatytimes.markheydon.me.uk/))
- [docs-internal](docs-internal) — contributor and developer documentation
- [.specify](.specify) — project [constitution](.specify/memory/constitution.md) and Spec Kit configuration
- [.github](.github) — CI/CD workflows, Dependabot, and repository automation

## Key Features

- Roast instruction calculation for beef, lamb, pork, chicken, and gammon
- Doneness selection where applicable, with food-safe defaults for poultry, pork, and gammon
- Serve-at backwards scheduling for planned meal timing
- Input summary above results showing which meat, weight, and doneness a calculation used — stable while the form is edited
- Responsive Tailwind CSS UI with a light teal-and-white calculator layout; Celsius primary with Fahrenheit equivalents in results
- No accounts, history, or persistence required for the primary calculate flow

## Development Workflow

Feature work in this repository can follow the Spec Kit workflow:

- `/speckit-specify`, `/speckit-plan`, `/speckit-tasks`, and `/speckit-implement` for structured feature delivery
- Spec Kit feature branches use `NNN-short-name` numbering (see [.specify/extensions/git/](.specify/extensions/git/))
- The [constitution](.specify/memory/constitution.md) (v2.0.0) sets binding quality gates across eight principles: separation of concerns, architectural discipline, testability, explicit error handling, user-facing consistency, security by design, traceability, and code quality

Contributors do not need Spec Kit — a well-described pull request is enough. See [CONTRIBUTING.md](CONTRIBUTING.md).

Stack-specific conventions live in [docs-internal/tech-stack.md](docs-internal/tech-stack.md).

## Coding Standards

The constitution in [.specify/memory/constitution.md](.specify/memory/constitution.md) sets the baseline for the project:

- Domain logic belongs in [src/MeatyTimes.Core](src/MeatyTimes.Core)
- Cooking-critical rules should be clearly commented and easy to audit
- Calculation changes require unit tests before merge
- Deterministic rules should remain documented and traceable through [src/MeatyTimes.Core/Rules/cooking-rules.json](src/MeatyTimes.Core/Rules/cooking-rules.json)

## Testing

MeatyTimes uses a two-layer test strategy. Tooling and project layout: [docs-internal/testing-standards.md](docs-internal/testing-standards.md). Coverage rules: constitution Principle III.

| Layer | Project | Purpose |
|-------|---------|---------|
| Unit | [tests/MeatyTimes.Core.Tests](tests/MeatyTimes.Core.Tests) | Domain logic and cooking calculations |
| Component | [tests/MeatyTimes.Web.Tests](tests/MeatyTimes.Web.Tests) | Blazor UI outcomes via bunit |

**Tooling standard**: xUnit v3, built-in `Assert` methods only, NSubstitute for mocks when needed. Do not introduce FluentAssertions, AwesomeAssertions, Shouldly, Moq, NUnit, or MSTest. Playwright is reserved for end-to-end user-journey tests when explicitly required.

Run the unit tests for the calculation engine:

```powershell
dotnet test tests/MeatyTimes.Core.Tests
```

Run the full suite:

```powershell
dotnet test MeatyTimes.slnx
# or
dotnet test
```

Cooking-critical behaviour should be covered by outcome-named tests and verified before merge.

## Contributing

Contributions are welcome. Please read [CONTRIBUTING.md](CONTRIBUTING.md) and [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md) before opening a pull request. For security issues, see [SECURITY.md](SECURITY.md).

For cooking-rule changes or new meat types, include:

- documented sources or rationale for the rule change
- unit tests in [tests/MeatyTimes.Core.Tests](tests/MeatyTimes.Core.Tests)
- user-facing examples or validation coverage where appropriate

Pull requests should pass the CI workflow in [.github/workflows/ci.yml](.github/workflows/ci.yml) (build, format check, tests) and the Aspire deploy validation in [.github/workflows/aspire-deploy-validate.yml](.github/workflows/aspire-deploy-validate.yml). Merges to `main` trigger CD deployment to Azure Container Apps via [.github/workflows/cd.yml](.github/workflows/cd.yml).

## License

MeatyTimes is released under the MIT License. See [LICENSE](LICENSE) for details.

Copyright © Mark Heydon.
