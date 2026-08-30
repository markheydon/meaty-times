# AGENTS.md

## Cursor Cloud specific instructions

MeatyTimes is a .NET 10 Aspire distributed app (a roast-cooking assistant). Standard build/test/run commands live in `README.md` and `CONTRIBUTING.md`; this section only captures non-obvious, environment-specific caveats.

### Services
- `MeatyTimes.AppHost` — Aspire orchestrator; launches the Web app and the Aspire dashboard.
- `MeatyTimes.Web` — Blazor Server + Tailwind CSS UI (the roast calculator); calls `MeatyTimes.Core` in-process.
- `MeatyTimes.Core` — domain logic; `MeatyTimes.ServiceDefaults` — OTel/health-check defaults.

### Non-obvious caveats
- The .NET 10 SDK is installed at `~/.dotnet` (not system-wide). `~/.bashrc` adds it to `PATH` and sets `DOTNET_ROOT`; a fresh non-login shell that skips `~/.bashrc` must call `dotnet` via `~/.dotnet/dotnet`.
- **HTTPS dev cert trust is required for local development.** Aspire's AppHost health-checks `webfrontend` over HTTPS. Without a trusted ASP.NET Core dev cert, the web app may never report healthy and `dotnet run` on the AppHost can hang. The dev cert is created and `SSL_CERT_DIR` is exported in `~/.bashrc` (`$HOME/.aspnet/dev-certs/trust:/etc/ssl/certs`) so `dotnet run` works. If you ever hit AppHost startup timeouts, re-run `dotnet dev-certs https --trust` and ensure `SSL_CERT_DIR` includes `~/.aspnet/dev-certs/trust`.
- Run the test suite with `dotnet test MeatyTimes.slnx` (33 tests: 20 Core unit, 13 Web component/service).
- Run the app with `dotnet run --project src/MeatyTimes.AppHost` (or `aspire run` if the Aspire CLI is installed — it is not preinstalled here). The dashboard and child services bind to dynamically assigned ports; check the AppHost console / dashboard for the actual `webfrontend` URL. It serves over HTTPS with the self-signed dev cert, so browsers show a certificate warning that must be bypassed.

### Testing standards

All automated tests MUST follow the tooling standard in `docs-internal/testing-standards.md` (constitution Principle III for what must be tested):

- **xUnit v3** with built-in `Assert` methods only
- **NSubstitute** for mocks/stubs when isolation is required (pin in `Directory.Packages.props`; reference per-project only when needed)
- **bunit** for Blazor component tests (`MeatyTimes.Web.Tests`)
- **Playwright** for end-to-end user journeys when explicitly required (not currently in use)

Do not introduce FluentAssertions, AwesomeAssertions, Shouldly, Moq, NUnit, or MSTest.
