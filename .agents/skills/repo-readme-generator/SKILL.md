---
name: repo-readme-generator
description: >-
  Generate or refresh README.md for MeatyTimes by analysing Spec Kit artefacts,
  governance docs, and repository layout. Use when the user asks to create, update,
  or regenerate the repository README, project documentation overview, or onboarding
  docs for new contributors.
disable-model-invocation: true
metadata:
  author: markheydon
---

# README Generator

Generate a comprehensive `README.md` for **MeatyTimes** by analysing repository-native sources of truth (Spec Kit artefacts and governance guidance).

## When to use

- User requests a new or updated repository `README.md`
- Onboarding docs need to reflect current architecture, workflow, or testing standards
- Spec Kit artefacts or constitution changed and the README should be brought in sync

## Workflow

Copy this checklist and track progress:

```
Task progress:
- [ ] Step 1: Scan Spec Kit feature artefacts
- [ ] Step 2: Review governance and contributor guidance
- [ ] Step 3: Draft README sections from sources
- [ ] Step 4: Write README.md with formatting and links
```

### Step 1: Scan Spec Kit feature artefacts

Scan feature folders under `specs/` (for example, `001-roast-calculator`), using each feature folder's standard files where present:

- `spec.md`
- `plan.md`
- `tasks.md`
- `research.md`
- `data-model.md`
- `quickstart.md`
- `contracts/*`

### Step 2: Review governance and contributor guidance

Read and extract relevant facts from:

- `.specify/memory/constitution.md`
- `.github/copilot-instructions.md` (if present)
- `AGENTS.md` (if present)
- `CONTRIBUTING.md` (if present)
- `docs/README.md` and relevant files under `docs/` for user-facing context when present
- `docs-internal/README.md` and relevant files under `docs-internal/` for contributor/developer context when present
- The current `README.md` (preserve useful badges, links, or wording where still accurate)

Also inspect `global.json`, `Directory.Packages.props`, `src/MeatyTimes.AppHost/`,
`.github/workflows/`, `.github/dependabot.yml`, and `docs/CNAME` (or `docs/index.md`)
for versions, CI/CD workflow filenames, deployment URLs, and docs site URLs.

### Step 3: Draft README sections

Ground each section in the sources above (and broader repository files where relevant).

#### Project Name and Description

- Project name: **MeatyTimes**
- Describe the app as a simple cooking assistant that calculates roasting instructions for joints of meat (meat type, weight, doneness) and optional serve-at scheduling.
- Source from feature `spec.md` files and the current `README.md`.

#### Technology Stack

- List primary technologies: .NET 10, C# / ASP.NET Core, Blazor Server, Tailwind CSS v4, Lucide icons, .NET Aspire, xUnit v3.
- Include SDK and key package versions from `global.json`, `Directory.Packages.props`, and feature `plan.md` files.
- Note central package management via `Directory.Packages.props`.

#### Project Architecture

High-level Aspire app layout:

- `MeatyTimes.AppHost` — Aspire orchestration
- `MeatyTimes.Core` — domain logic, cooking rules, calculation engine
- `MeatyTimes.Web` — Blazor Server + Tailwind CSS UI (calls Core in-process)
- `MeatyTimes.ServiceDefaults` — OpenTelemetry and health checks

Source from feature `plan.md`, `data-model.md`, and `contracts/*` under `specs/`.

#### Getting Started

- Prerequisites: .NET SDK (see `global.json`), Aspire CLI (recommended).
- Local run commands:

  ```powershell
  aspire run
  # or
  dotnet run --project src/MeatyTimes.AppHost
  ```

- Open the **webfrontend** endpoint from the Aspire dashboard.
- Source from `README.md`, feature `quickstart.md` files, and `src/MeatyTimes.AppHost/`.

#### Project Structure

Brief overview of repository layout:

- `src/` — application projects
- `tests/` — `MeatyTimes.Core.Tests` (unit) and `MeatyTimes.Web.Tests` (Blazor component)
- `specs/` — Spec Kit feature specifications and plans
- `docs/` — end-user documentation that can inform how the project is presented
- `docs-internal/` — internal contributor documentation that can inform development guidance
- `.specify/` — project constitution and Spec Kit configuration
- `.github/` — CI, Dependabot, and prompts

#### Key Features

- Roast instruction calculator (beef, lamb, pork, chicken, gammon).
- Doneness selection where applicable; food-safe defaults for poultry and pork.
- Serve-at backwards scheduling.
- Responsive Tailwind CSS UI for mobile, tablet, and desktop.
- Source from feature `spec.md` and `contracts/*` files.

#### Development Workflow

- Feature work uses Spec Kit (`/speckit-specify`, `/speckit-plan`, `/speckit-tasks`, `/speckit-implement`).
- Feature branches follow `NNN-short-name` numbering (see `.specify/extensions/git/`).
- Constitution gates in `.specify/memory/constitution.md` govern separation of concerns, testing, UX, security, traceability, and code quality (v2.0.0, eight principles).
- Source from `CONTRIBUTING.md` (if present), feature `tasks.md`, and the constitution.

#### Coding Standards

- Domain logic lives in `MeatyTimes.Core`; cooking-critical code requires comments explaining rules and intent.
- Cooking calculation changes require unit tests (red-green-refactor).
- Deterministic calculations with documented rule sources in `cooking-rules.json`.
- Source from `.specify/memory/constitution.md` and `.github/copilot-instructions.md` (if present).

#### Testing

- Unit tests: `dotnet test tests/MeatyTimes.Core.Tests`
- Full suite: `dotnet test`
- Cooking-critical behaviour must be covered by outcome-named tests per constitution Principle III.
- Source from feature `quickstart.md`, `tests/`, and the constitution.

#### Contributing

- Reference `CONTRIBUTING.md` and `CODE_OF_CONDUCT.md` if present.
- New meat types or rule changes require documented sources, unit tests, and user-facing examples.
- Pull requests should pass CI (build, format check, tests).

#### Licence

- MIT Licence — see `LICENSE` (Copyright Mark Heydon).

#### Badges and quick stats

Place badges immediately under the `# MeatyTimes` title, grouped in up to three rows.
Derive URLs from repository facts — do not invent badge services the project does not use.

**Row 1 — build health** (always include when workflows exist):

| Badge | Source |
|-------|--------|
| CI | `.github/workflows/ci.yml` → `actions/workflows/ci.yml/badge.svg?branch=main` |
| CD / deploy | `cd.yml` or equivalent deploy workflow, same pattern |
| .NET SDK | `global.json` `sdk.version` |
| Licence | `LICENSE` file (MIT) |

**Row 2 — discoverability** (include for public GitHub repos):

| Badge | shields.io pattern |
|-------|-------------------|
| Live demo | Static badge linking to production URL from `docs/index.md` or `docs-internal/deployment.md` |
| Docs | Static badge linking to `docs/CNAME` custom domain, or GitHub Pages `/docs` path |
| GitHub stars | `github/stars/{owner}/{repo}?style=flat-square` |
| Open issues | `github/issues/{owner}/{repo}?style=flat-square` |
| Last commit | `github/last-commit/{owner}/{repo}/main?style=flat-square` |

**Row 3 — tech stack** (pick badges that match `Directory.Packages.props` and `docs-internal/tech-stack.md`):

- Blazor Server, .NET Aspire (include major version from packages), Tailwind CSS, xUnit
- Dependabot enabled when `.github/dependabot.yml` exists

**Do not add** unless the repo actually publishes them:

- Code coverage / Codecov / Coveralls (no coverage upload in CI today)
- SonarCloud / Code Climate quality gates
- Docker Hub pulls (unless images are published to a registry)
- Static “tests passing” counts that will go stale — prefer the CI badge; optional compact stats table is fine if refreshed from `dotnet test` at generation time

**Quick stats table** (optional, directly under badges):

A two-column markdown table with durable facts: live app URL, docs URL, test count
(from `dotnet test MeatyTimes.slnx`), and hosting target (e.g. Azure Container Apps).
Replace redundant prose “Live app:” lines when this table is present.

Preserve existing badges from the current `README.md` when still accurate.

### Step 4: Write README.md

Update the repository root `README.md` with:

- Clear headings and subheadings
- Code blocks for commands
- Lists for readability
- Links to `specs/` documentation and the constitution
- Badges and optional quick-stats table per **Badges and quick stats** above

Keep the README concise yet informative, focusing on what new developers or users need to know about MeatyTimes.

## Output

- Primary deliverable: updated `README.md` at the repository root
- Summarise for the user which sources drove major sections and any gaps where documentation was missing
