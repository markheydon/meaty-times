# Tech stack

Stack and layout conventions for MeatyTimes. These are **not** Spec Kit
constitution principles: they may change if the platform changes. Architectural
rules (domain isolated from UI, simplest design, no speculative abstractions)
live in [`.specify/memory/constitution.md`](../.specify/memory/constitution.md).

Canonical published overview: [README.md](../README.md). Package versions are
pinned in [Directory.Packages.props](../Directory.Packages.props) and the SDK in
[global.json](../global.json).

## Current stack

- .NET SDK 10 (see `global.json`)
- C# / ASP.NET Core, Blazor Server, Tailwind CSS v4 standalone CLI, Lucide icons
- .NET Aspire for local orchestration and Azure Container Apps deployment
- OpenTelemetry via `MeatyTimes.ServiceDefaults`
- Central package management (`Directory.Packages.props`)

## Projects

| Project | Role |
|---------|------|
| `src/MeatyTimes.AppHost` | Aspire orchestrator |
| `src/MeatyTimes.Web` | Blazor Server + Tailwind CSS UI; calls Core in-process |
| `src/MeatyTimes.Core` | Domain models, cooking rules, calculation engine |
| `src/MeatyTimes.ServiceDefaults` | OTel and health-check defaults |
| `tests/MeatyTimes.Core.Tests` | Domain unit tests |
| `tests/MeatyTimes.Web.Tests` | bunit component tests |

Design and contracts for the roast calculator live under
[specs/001-roast-calculator](../specs/001-roast-calculator).

## Runtime notes for agents

- Prefer `dotnet run --project src/MeatyTimes.AppHost` or `aspire run`.
- Dashboard and child services bind to dynamic ports; use the AppHost console
  or Aspire dashboard for the `webfrontend` URL.
- HTTPS dev-cert trust is required for AppHost health checks; see `AGENTS.md`.
- Linting/formatting: CI runs `dotnet format --verify-no-changes`.
- Tailwind CSS is built via MSBuild `BeforeBuild` and the standalone CLI at
  `tools/tailwind/tailwindcss` (linux-x64). The binary is not committed; run
  `tools/tailwind/download.sh` or `dotnet build` on `MeatyTimes.Web` to fetch it.
  Replace the download script asset name for other OS targets as needed.

Cooking rules that must stay documented and traceable are stored in
`src/MeatyTimes.Core/Rules/cooking-rules.json`.
