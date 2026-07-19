---
name: repo-readme-generator
description: 'Intelligent README.md generation prompt that analyses MeatyTimes project documentation and creates comprehensive repository documentation. Scans Spec Kit artefacts, constitution, and source layout to extract project purpose, technology stack, architecture, development workflow, coding standards, and testing approaches.'
model: MAI-Code-1-Flash
tools: [execute, read, edit, search, web, agent, todo]
---

# README Generator Prompt

Generate a comprehensive README.md for the **MeatyTimes** repository by analysing repository-native sources of truth (Spec Kit artefacts and governance guidance). Follow these steps.

1. Scan the Spec Kit feature artefacts across all feature folders under `specs/` (for example, `001-roast-calculator`), using each feature folder's standard files where present.
   - `spec.md`
   - `plan.md`
   - `tasks.md`
   - `research.md`
   - `data-model.md`
   - `quickstart.md`
   - `contracts/*`

2. Review repository governance and contributor guidance.
   - `.specify/memory/constitution.md`
   - `.github/copilot-instructions.md` (if present)
   - `AGENTS.md` (if present)
   - `CONTRIBUTING.md` (if present)
   - `docs/README.md` and relevant files under `docs/` for user-facing context when present
   - `docs-internal/README.md` and relevant files under `docs-internal/` for contributor/developer context when present

3. Create a README.md with the following sections, grounding each section in the sources above (and broader repository files where relevant).

## Project Name and Description
- Project name: **MeatyTimes**
- Describe the app as a simple cooking assistant that calculates roasting instructions for joints of meat (meat type, weight, doneness) and optional serve-at scheduling.
- Source from feature `spec.md` files and the current `README.md`.

## Technology Stack
- List primary technologies: .NET 10, C# / ASP.NET Core, Blazor Server, MudBlazor, .NET Aspire, xUnit v3.
- Include SDK and key package versions from `global.json`, `Directory.Packages.props`, and feature `plan.md` files.
- Note central package management via `Directory.Packages.props`.

## Project Architecture
- High-level distributed app layout:
  - `MeatyTimes.AppHost` — Aspire orchestration
  - `MeatyTimes.ApiService` — minimal API for roast calculation
  - `MeatyTimes.Core` — domain logic, cooking rules, calculation engine
  - `MeatyTimes.Web` — MudBlazor Blazor UI
  - `MeatyTimes.ServiceDefaults` — OpenTelemetry and health checks
- Source from feature `plan.md`, `data-model.md`, and `contracts/*` under `specs/`.

## Getting Started
- Prerequisites: .NET SDK (see `global.json`), Aspire CLI (recommended).
- Local run commands:
  ```powershell
  aspire run
  # or
  dotnet run --project src/MeatyTimes.AppHost
  ```
- Open the **webfrontend** endpoint from the Aspire dashboard.
- Source from `README.md`, feature `quickstart.md` files, and `src/MeatyTimes.AppHost/`.

## Project Structure
- Brief overview of repository layout:
  - `src/` — application projects
  - `tests/` — `MeatyTimes.Core.Tests` (unit) and `MeatyTimes.Web.Tests` (Blazor component)
  - `specs/` — Spec Kit feature specifications and plans
  - `docs/` — end-user documentation that can inform how the project is presented
  - `docs-internal/` — internal contributor documentation that can inform development guidance
  - `.specify/` — project constitution and Spec Kit configuration
  - `.github/` — CI, Dependabot, and prompts

## Key Features
- Roast instruction calculator (beef, lamb, pork, chicken, gammon).
- Doneness selection where applicable; food-safe defaults for poultry and pork.
- Serve-at backwards scheduling.
- Responsive MudBlazor UI for mobile, tablet, and desktop.
- Source from feature `spec.md` and `contracts/*` files.

## Development Workflow
- Feature work uses Spec Kit (`/speckit-specify`, `/speckit-plan`, `/speckit-tasks`, `/speckit-implement`).
- Feature branches follow `NNN-short-name` numbering (see `.specify/extensions/git/`).
- Constitution gates in `.specify/memory/constitution.md` govern code quality, testing, UX, security, cooking accuracy, and simplicity.
- Source from `CONTRIBUTING.md` (if present), feature `tasks.md`, and the constitution.

## Coding Standards
- Domain logic lives in `MeatyTimes.Core`; cooking-critical code requires comments explaining rules and intent.
- Cooking calculation changes require unit tests (red-green-refactor).
- Deterministic calculations with documented rule sources in `cooking-rules.json`.
- Source from `.specify/memory/constitution.md` and `.github/copilot-instructions.md` (if present).

## Testing
- Unit tests: `dotnet test tests/MeatyTimes.Core.Tests`
- Full suite: `dotnet test`
- Cooking-critical behaviour must be covered by outcome-named tests per constitution Principle II.
- Source from feature `quickstart.md`, `tests/`, and the constitution.

## Contributing
- Reference `CONTRIBUTING.md` and `CODE_OF_CONDUCT.md` if present.
- New meat types or rule changes require documented sources, unit tests, and user-facing examples.
- Pull requests should pass CI (build, format check, tests).

## Licence
- MIT Licence — see `LICENSE` (Copyright Mark Heydon).

Format the README with proper Markdown, including the following.
- Clear headings and subheadings.
- Code blocks for commands.
- Lists for readability.
- Links to `specs/` documentation and constitution.
- Optional badges (build status, .NET version, licence) if CI workflow name and branch are known.

Keep the README concise yet informative, focusing on what new developers or users need to know about MeatyTimes.
