# CI requirements for Aspire integration tests

The GitHub Actions workflow in [.github/workflows/ci.yml](../.github/workflows/ci.yml) runs unit tests and Aspire AppHost integration tests on `ubuntu-latest`.

## HTTPS development certificate (required)

Aspire integration tests start the full AppHost and probe child services over HTTPS. On Linux CI runners the ASP.NET Core development certificate must be trusted before integration tests run.

The workflow:

1. Runs `MeatyTimes.Core.Tests` and `MeatyTimes.Web.Tests` first (no AppHost).
2. Cleans and trusts the dev certificate via `dotnet dev-certs https --clean` and `dotnet dev-certs https --trust`.
3. Sets `SSL_CERT_DIR` to `$HOME/.aspnet/dev-certs/trust:/etc/ssl/certs`.
4. Runs [`tests/MeatyTimes.AppHost.Tests`](../tests/MeatyTimes.AppHost.Tests/) only after cert trust succeeds.

Without this step, `StartAsync` times out because `apiservice` never becomes healthy and `webfrontend` (which `WaitFor`s the API) never starts. See [AGENTS.md](../AGENTS.md) and [Aspire troubleshooting: untrusted localhost certificate](https://learn.microsoft.com/en-us/dotnet/aspire/troubleshooting/untrusted-localhost-certificate).

On .NET SDK 10.x, `dotnet dev-certs https --trust` may exit with code `4` (partial trust) even when the certificate is usable. The workflow treats exit codes `0` and `4` as success.

If integration tests fail in CI, download the `dcp-logs` workflow artifact for DCP diagnostics.

## GitHub Actions environment

GitHub Actions sets `CI=true` automatically. Integration tests use longer startup timeouts when `CI` is set.

## Azure credentials (optional, future use)

MeatyTimes deploys to Azure Container Apps via `aspire deploy`. The AppHost includes `AddAzureContainerAppEnvironment("aca-env")` for deployment metadata. **Current integration tests do not provision or call Azure resources** and do not require Azure authentication.

When a future test or AppHost scenario needs Azure, configure these GitHub Actions secrets and pass them to the integration test step:

| Variable | Description |
| --- | --- |
| `AZURE_CLIENT_ID` | Service principal application (client) ID |
| `AZURE_TENANT_ID` | Azure AD tenant ID |
| `AZURE_CLIENT_SECRET` | Service principal client secret |
| `AZURE_SUBSCRIPTION_ID` | Azure subscription ID |

See [Testing in CI/CD pipelines | Aspire](https://aspire.dev/testing/testing-in-ci/) for the standard `DefaultAzureCredential` environment-variable pattern.
