<!--
Sync Impact Report
==================
Version change: (none) → 1.0.0
Modified principles: N/A (initial adoption, adapted from BillDrift v1.2.0)
Added sections:
  - Core Principles I–VI
  - Domain Constraints
  - Development Workflow & Quality Gates
  - Governance
Removed sections: None
Templates requiring updates:
  - .specify/templates/plan-template.md ✅ updated (Constitution Check gates + Principle VI reference)
  - .specify/templates/spec-template.md ✅ no change (no implementation-specific constraints)
  - .specify/templates/tasks-template.md ✅ updated (Polish phase simplicity verification)
  - .specify/templates/commands/*.md — N/A (no command files present)
  - README.md ✅ no changes required
Follow-up TODOs: None
-->

# MeatyTimes Constitution

## Core Principles

### I. Code Quality & Maintainability

All code MUST be clear, reviewable, and structured for long-term maintenance in a
simple cooking-assistant app.

- Domain logic (meat-type rules, weight-to-time calculations, temperature profiles,
  resting guidance, instruction sequencing) MUST live in well-named modules with single,
  explicit responsibilities.
- Public interfaces between modules MUST be typed and documented; implicit coupling
  across meat types or calculation paths is prohibited.
- Code comments are REQUIRED, not optional. Every module, public interface, and
  non-trivial algorithm MUST include comments that explain intent, cooking rules,
  and non-obvious behavior.
- Cooking calculation logic (time-per-kg rules, temperature steps, doneness thresholds,
  resting durations, meat-specific exceptions) MUST be commented so contributors and
  users can verify correctness without reverse-engineering.
- Comments MUST explain why a decision was made, not merely restate what the code
  does; redundant comments that duplicate obvious code are discouraged, but
  absence of required comments is prohibited.
- Linting and formatting MUST pass in CI before merge; style drift is not accepted.
- Complexity beyond the simplest working design MUST be documented in the feature plan
  Complexity Tracking table with rejected alternatives.
- Dead code, commented-out blocks, and unused dependencies MUST be removed before merge.

**Rationale**: MeatyTimes exists to replace guesswork with trustworthy roasting
guidance. Maintainable, well-commented code keeps cooking rules inspectable and
reduces the risk of silently wrong instructions that could ruin a meal.

### II. Testing Standards (NON-NEGOTIABLE)

Cooking-critical behavior MUST be proven by automated tests before it ships.

- Roasting calculation logic (cooking time, temperature steps, resting duration,
  doneness adjustments) MUST have unit tests covering normal cases, edge cases, and
  known meat-type rule variations.
- Instruction generation (step ordering, unit display, weight-boundary behaviour)
  MUST have tests that assert user-facing outcomes, not implementation details.
- For cooking-critical features, tests MUST be written or updated first, MUST fail
  before implementation, and MUST pass before merge (red-green-refactor).
- Test names and assertions MUST describe business outcomes (e.g., "returns 20-minute
  rest for a 2 kg beef joint at medium-rare"), not internal method names.
- Interfaces SHOULD NOT be created solely to enable mocking; domain logic SHOULD
  normally be tested through concrete types.

**Rationale**: Incorrect cooking times or temperatures directly cause undercooked or
overcooked food. Tests are the primary safety net for a domain where users otherwise
rely on inconsistent online sources or memory.

### III. Consistent User Experience

The product MUST feel coherent, predictable, and easy to use for home cooks who roast
joints infrequently.

- Terminology MUST be consistent across UI, API responses, logs, and documentation
  (e.g., "doneness", "resting time", "initial temperature", "reduce-to temperature").
- Results MUST present clear, step-by-step roasting instructions: oven settings,
  timing, temperature changes, and resting guidance in a fixed, scannable order.
- Input validation MUST give actionable feedback (e.g., unsupported meat type,
  out-of-range weight) without exposing stack traces or internal errors.
- Error, empty, and loading states MUST be handled consistently; failures MUST tell
  the user what went wrong and what to do next.
- Accessibility and responsive layout MUST be considered for the primary calculate-and-
  display workflow; new UI MUST reuse established components and patterns rather than
  one-off implementations.
- The experience MUST prioritise speed and simplicity: users SHOULD reach reliable
  instructions in minimal steps without navigating recipe libraries or meal planners.

**Rationale**: Users adopt MeatyTimes to avoid searching cookbooks or the web every
time they roast. An inconsistent or cluttered UX recreates the confusion the app is
meant to eliminate.

### IV. Security by Design

Security MUST be treated as a requirement, not a polish item, even for a simple
cooking assistant.

- Secrets (API keys, tokens, connection strings) MUST NEVER be committed, logged, or
  returned in API responses; use environment variables or a secrets manager when
  external services are introduced.
- User-supplied input (meat type, weight, doneness) MUST be validated and sanitised
  before use in calculations or rendered output.
- Dependencies MUST be kept current; known high/critical vulnerabilities in direct
  dependencies MUST be remediated or explicitly waived with documented risk acceptance.
- If authentication or persistence is added in future versions, access controls MUST
  protect user data from the outset; retrofitted security is not acceptable.

**Rationale**: Even a lightweight app accumulates trust. Input handling flaws or
neglected dependencies can undermine user confidence and create avoidable risk as
the product grows.

### V. Cooking Accuracy & Source Transparency

Roasting instructions MUST be correct, traceable, and honest about their basis.

- Calculation algorithms MUST be deterministic for a given input; the same meat type,
  weight, and doneness MUST always produce the same instruction set.
- Every instruction set MUST be explainable: which rule fired, which weight band
  applied, and how temperature and resting guidance were derived.
- Cooking rules MUST be sourced from documented references (cookbooks, food-safety
  guidance, or project-approved references); magic numbers without documented
  provenance MUST NOT ship.
- When multiple authoritative sources disagree, the chosen rule MUST be documented
  with rationale; silent averaging or undocumented compromise is prohibited.
- User-facing surfaces MUST NOT imply official endorsement by food brands, chefs, or
  publishers unless a formal partnership exists.

**Rationale**: MeatyTimes replaces guesswork only if users can trust the output.
Traceable, documented rules make errors correctable and build confidence for occasional
home cooks.

### VI. Pragmatic Simplicity

MeatyTimes prioritises simple, understandable solutions over architectural purity.

- Implementations MUST begin with the simplest design capable of meeting the current
  feature specification.
- Abstractions MUST NOT be introduced solely for anticipated future requirements.
- Interfaces, providers, factories, strategies, pipelines, mediators, or additional
  project boundaries require at least one of the following:
  - More than one active implementation.
  - A proven testing requirement that cannot reasonably be satisfied through concrete types.
  - Existing duplication that would otherwise remain.
  - Isolation of external dependencies.
- Where multiple designs are possible, the design with the lowest conceptual
  complexity SHOULD be preferred.
- Future roadmap items (recipe management, meal planning, user accounts) SHOULD be
  represented in specifications and documentation rather than premature implementation.

**Rationale**: Over-engineering slows delivery and obscures cooking logic. Simplicity
keeps the codebase approachable and aligned with the product goal of fast, reliable
roasting guidance.

## Domain Constraints

- **Primary users**: Home cooks who roast large joints of meat occasionally and need
  reliable temperature, timing, and resting guidance without consulting multiple sources.
- **v0.1 scope**: Calculate and display roasting instructions from meat type, weight,
  and desired doneness; prioritise simplicity, speed, and accuracy.
- **Out of scope (v0.1)**: Recipe management, meal planning, shopping lists, social
  sharing, multi-course menus, unattended cooking automation.
- **Accuracy expectation**: Instructions MUST reflect documented roasting conventions;
  food-safety minimums MUST NOT be compromised for convenience.
- **License & openness**: The project is open source; contributions MUST preserve
  clarity of cooking logic and test coverage expectations defined in this constitution.

## Development Workflow & Quality Gates

- Every feature plan MUST include a Constitution Check gate (pre-research and
  post-design) verifying compliance with Principles I–VI.
- Pull requests MUST not merge with failing CI, missing tests for cooking-critical
  changes, or unresolved security findings above the project's accepted threshold.
- Code review MUST explicitly confirm: calculation correctness, instruction clarity
  for home cooks, source documentation for new or changed cooking rules, adequate
  code comments on new or modified cooking-critical logic, public interfaces, and
  non-obvious implementation choices, and that abstractions introduced satisfy
  Principle VI.
- New meat types or materially changed calculation rules MUST include unit tests,
  documented source references, and user-facing examples before release.
- Runtime development guidance lives in feature `quickstart.md` files and `README.md`;
  agents and contributors MUST consult the current feature plan for stack-specific
  commands.

## Governance

This constitution supersedes ad-hoc practices for MeatyTimes feature work. When
implementation guidance conflicts with a principle here, the constitution wins unless
formally amended.

- Amendments MUST be documented in `.specify/memory/constitution.md` with an updated
  Sync Impact Report, version bump, and `LAST_AMENDED_DATE`.
- Version increments follow semantic rules: MAJOR for backward-incompatible principle
  removals or redefinitions; MINOR for new principles or materially expanded guidance;
  PATCH for clarifications and non-semantic refinements.
- All pull requests and feature plans MUST verify compliance with the current
  constitution version before merge or implementation begins.
- Complexity beyond the simplest working design MUST be justified in the feature plan
  Complexity Tracking table.

**Version**: 1.0.0 | **Ratified**: 2026-07-02 | **Last Amended**: 2026-07-02
