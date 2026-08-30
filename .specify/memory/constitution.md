<!--
Sync Impact Report
==================
Version change: 1.1.1 → 2.0.0
Modified principles:
  - I. Code Quality & Maintainability → I. Separation of Concerns
      (quality rules split; stack-specific module examples removed)
  - II. Testing Standards → III. Testability
      (tooling, libraries, and test-project names migrated)
  - III. Consistent User Experience → IV. Explicit Error Handling
      (UX/terminology rules moved into V. User-Facing Consistency)
  - IV. Security by Design → VI. Security by Design (content retained)
  - V. Cooking Accuracy & Source Transparency → VII. Traceability
  - VI. Pragmatic Simplicity → II. Architectural Discipline
      (renumbered; YAGNI rules retained without stack examples)
Added principles:
  - II. Architectural Discipline
  - V. User-Facing Consistency
Added sections:
  - None in this file (Quality Gates retained as section 2)
Removed sections:
  - Domain Constraints (migrated to docs-internal/product-scope.md)
Migrated out of constitution (retained in docs-internal/):
  - Testing tooling, libraries, layering, and forbidden packages
    → docs-internal/testing-standards.md
  - Stack, project layout, and contributor run/test conventions
    → docs-internal/tech-stack.md
  - v0.1 product scope, primary users, in/out of scope
    → docs-internal/product-scope.md
Templates requiring updates:
  - Dependent Spec Kit templates are not modified by this command
  - Existing specs that cite old principle numbers (I–VI) need a later pass
Follow-up TODOs: None
-->

# MeatyTimes Constitution

## Core Principles

### I. Separation of Concerns

Domain behaviour MUST be isolated from presentation, orchestration, and I/O.

- Cooking rules, calculations, and instruction sequencing MUST live in dedicated
  domain modules with a single, explicit responsibility.
- Presentation, hosting, and infrastructure MUST consume domain outputs; they MUST
  NOT embed or duplicate cooking rules.
- Public interfaces between modules MUST be typed and documented.
- Implicit coupling across independent domain paths is prohibited.

**Rationale**: Mixing calculation rules with UI or hosting makes errors hard to find
and untestable in isolation. Clear boundaries keep roasting guidance inspectable.

### II. Architectural Discipline

Implementations MUST stay at the simplest design that meets the current specification.

- Work MUST start from the simplest design capable of meeting the current feature spec.
- Abstractions MUST NOT be introduced solely for anticipated future requirements.
- Interfaces, providers, factories, strategies, pipelines, mediators, or extra project
  boundaries require at least one of: more than one active implementation; a proven
  testing need that concrete types cannot reasonably satisfy; existing duplication
  that would otherwise remain; isolation of an external dependency.
- Complexity beyond the simplest working design MUST be recorded in the feature plan
  Complexity Tracking table with rejected alternatives.
- Future ideas MUST be recorded in specifications and documentation, not implemented
  ahead of a current requirement.

**Rationale**: Extra layers hide cooking logic and slow review. Discipline keeps the
system small enough that a wrong instruction can be found and fixed.

### III. Testability (NON-NEGOTIABLE)

Cooking-critical behaviour MUST be proven by automated tests before it ships.

- Roasting calculation logic MUST have automated tests covering normal cases, edge
  cases, and known rule variations.
- Instruction generation MUST be tested by asserting user-facing outcomes, not
  internal method structure.
- For cooking-critical features, tests MUST be written or updated first, MUST fail
  before implementation, and MUST pass before merge.
- Test names and assertions MUST describe business outcomes, not internal names.
- Types MUST NOT be introduced solely to enable mocking; domain logic MUST normally
  be exercised through the same types production code uses.
- Tooling, libraries, and test-project layout live in
  `docs-internal/testing-standards.md` and MUST be followed; they are not restated here
  so a stack change does not require a constitution amendment.

**Rationale**: Wrong times or temperatures cause undercooked or overcooked food.
Tests are the check that a spec or PR can pass or fail without arguing taste.

### IV. Explicit Error Handling

Failures MUST be visible, explained, and safe.

- Invalid input MUST be rejected with actionable feedback.
- Internal errors, stack traces, and implementation details MUST NOT be shown to
  users.
- Error, empty, and in-progress states MUST each have defined behaviour; silent
  failure is prohibited.
- User-facing failure messages MUST state what went wrong and what to do next.

**Rationale**: A cooking assistant that fails quietly is worse than no assistant.
Callers and cooks need a clear next step, not a blank or crashed screen.

### V. User-Facing Consistency

User-facing behaviour MUST be coherent and checkable across surfaces.

- Terminology MUST be consistent across UI, API responses, logs, and documentation.
- Results MUST present roasting instructions in a fixed, scannable order covering
  oven settings, timing, temperature changes, and resting guidance.
- New UI MUST reuse established patterns for the primary calculate-and-display
  workflow rather than one-off implementations.
- Accessibility and responsive layout MUST be addressed for that primary workflow.
- The path to instructions MUST stay short: users MUST reach results without being
  required to use recipe libraries, accounts, or meal planners.

**Rationale**: Occasional cooks will not learn a new vocabulary each visit. Consistent
wording and order are the difference between trust and another web search.

### VI. Security by Design

Security MUST be treated as a requirement, not a polish item.

- Secrets MUST NEVER be committed, logged, or returned in responses; environment
  variables or a secrets manager MUST be used when external services are introduced.
- User-supplied input MUST be validated and sanitised before use in calculations or
  rendered output.
- Known high or critical vulnerabilities in direct dependencies MUST be remediated
  or explicitly waived with documented risk acceptance.
- If authentication or persistence is added, access controls MUST protect user data
  from the outset.

**Rationale**: Input handling flaws and neglected dependencies create avoidable risk
even in a small app. Security is cheaper when it is designed in.

### VII. Traceability

Roasting instructions MUST be correct, deterministic, and honest about their basis.

- The same meat type, weight, and doneness MUST always produce the same instruction set.
- Every instruction set MUST be explainable: which rule fired, which weight band
  applied, and how temperature and resting guidance were derived.
- Cooking rules MUST cite documented references; magic numbers without documented
  provenance MUST NOT ship.
- When sources disagree, the chosen rule MUST be documented with rationale; silent
  averaging or undocumented compromise is prohibited.
- User-facing surfaces MUST NOT imply endorsement by brands, chefs, or publishers
  unless a formal partnership exists.
- Food-safety minimums MUST NOT be weakened for convenience.

**Rationale**: Trust depends on being able to answer why an instruction exists. If a
rule cannot be traced, it cannot be reviewed.

### VIII. Code Quality

Code MUST be reviewable, commented, and free of avoidable residue.

- Every module, public interface, and non-trivial algorithm MUST include comments that
  explain intent and non-obvious behaviour.
- Cooking calculation logic MUST be commented so a reviewer can verify the rule
  without reverse-engineering.
- Comments MUST explain why a decision was made, not merely restate the code.
- Linting and formatting MUST pass in CI before merge.
- Dead code, commented-out blocks, and unused dependencies MUST be removed before merge.

**Rationale**: MeatyTimes replaces guesswork only if contributors can audit the rules.
Uncommented or leftover code is a defect, not a style preference.

## Quality Gates

- Every feature plan MUST include a Constitution Check gate (pre-research and
  post-design) verifying compliance with Principles I–VIII.
- Pull requests MUST not merge with failing CI, missing tests for cooking-critical
  changes, or unresolved security findings above the project's accepted threshold.
- Code review MUST confirm: calculation correctness; instruction clarity; source
  documentation for new or changed cooking rules; required comments; explicit error
  handling; and that any new abstraction satisfies Principle II.
- New meat types or materially changed calculation rules MUST include automated tests,
  documented source references, and user-facing examples before release.
- Stack-specific commands, package versions, and runtime setup live in
  `docs-internal/tech-stack.md`, `README.md`, and feature `quickstart.md` files.

## Governance

This constitution supersedes ad-hoc practices for MeatyTimes feature work. When
implementation guidance conflicts with a principle here, the constitution wins unless
formally amended.

- Amendments MUST be documented in `.specify/memory/constitution.md` with an updated
  Sync Impact Report, version bump, and `LAST_AMENDED_DATE`.
- Version increments follow semantic rules: MAJOR for backward-incompatible principle
  removals or redefinitions; MINOR for new principles or materially expanded guidance;
  PATCH for clarifications and non-semantic refinements.
- Stack, testing-tool, and product-scope changes that do not alter these principles MUST
  be made in `docs-internal/` (or the relevant spec) rather than in this file.
- All pull requests and feature plans MUST verify compliance with the current
  constitution version before merge or implementation begins.
- A rule belongs here only if a reviewer can answer yes or no against a spec or PR
  without depending on a particular library, framework, or UI kit.

**Version**: 2.0.0 | **Ratified**: 2026-07-02 | **Last Amended**: 2026-08-30
