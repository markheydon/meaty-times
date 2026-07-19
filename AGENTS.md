# AGENTS.md

## Cursor Cloud specific instructions

MeatyTimes is a .NET 10 Aspire distributed app (a roast-cooking assistant). Standard build/test/run commands live in `README.md` and `CONTRIBUTING.md`; this section only captures non-obvious, environment-specific caveats.

### Services
- `MeatyTimes.AppHost` — Aspire orchestrator; launches the API + Web and the Aspire dashboard.
- `MeatyTimes.ApiService` — minimal API for roast calculation/scheduling (`/api/meats`, `/api/roast/calculate`, `/api/roast/schedule`).
- `MeatyTimes.Web` — Blazor Server + MudBlazor UI (the roast calculator).
- `MeatyTimes.Core` — domain logic; `MeatyTimes.ServiceDefaults` — OTel/health-check defaults.

### Non-obvious caveats
- The .NET 10 SDK is installed at `~/.dotnet` (not system-wide). `~/.bashrc` adds it to `PATH` and sets `DOTNET_ROOT`; a fresh non-login shell that skips `~/.bashrc` must call `dotnet` via `~/.dotnet/dotnet`.
- **HTTPS dev cert trust is required.** Aspire's AppHost health-checks the API over HTTPS. Without a trusted ASP.NET Core dev cert, the API never reports healthy, so `webfrontend` (which uses `WaitFor(apiService)`) never starts and `dotnet run` on the AppHost hangs / the integration tests in `tests/MeatyTimes.AppHost.Tests` time out at `StartAsync`. The dev cert is created and `SSL_CERT_DIR` is exported in `~/.bashrc` (`$HOME/.aspnet/dev-certs/trust:/etc/ssl/certs`) so both `dotnet run` and `dotnet test` work. If you ever hit AppHost startup timeouts, re-run `dotnet dev-certs https --trust` and ensure `SSL_CERT_DIR` includes `~/.aspnet/dev-certs/trust`.
- The full test suite (`dotnet test MeatyTimes.slnx`) includes Aspire integration tests that spin up the whole app via DCP; give them time (~20s) and expect no external containers (this app has no databases/containers).
- Run the app with `dotnet run --project src/MeatyTimes.AppHost` (or `aspire run` if the Aspire CLI is installed — it is not preinstalled here). The dashboard and child services bind to dynamically assigned ports; check the AppHost console / dashboard for the actual `webfrontend` URL. It serves over HTTPS with the self-signed dev cert, so browsers show a certificate warning that must be bypassed.

### Testing standards

All automated tests MUST follow the tooling standard in `.specify/memory/constitution.md` Principle II:

- **xUnit v3** with built-in `Assert` methods only
- **NSubstitute** for mocks/stubs when isolation is required (pin in `Directory.Packages.props`; reference per-project only when needed)
- **bunit** for Blazor component tests (`MeatyTimes.Web.Tests`)
- **Aspire.Hosting.Testing** for integration smoke tests (`MeatyTimes.AppHost.Tests`)
- **Playwright** for end-to-end user journeys when explicitly required (not currently in use)

Do not introduce FluentAssertions, AwesomeAssertions, Shouldly, Moq, NUnit, or MSTest.
