# CI requirements

The GitHub Actions workflow in [.github/workflows/ci.yml](../.github/workflows/ci.yml) runs unit and component tests on `ubuntu-latest` via `dotnet test MeatyTimes.slnx`.

## HTTPS development certificate (local development only)

Aspire AppHost health-checks the API over HTTPS during local development. Without a trusted ASP.NET Core development certificate, `apiservice` may never report healthy and `webfrontend` (which `WaitFor`s the API) may not start.

For local `dotnet run --project src/MeatyTimes.AppHost`, trust the dev certificate:

```powershell
dotnet dev-certs https --trust
```

See [AGENTS.md](../AGENTS.md) and [Aspire troubleshooting: untrusted localhost certificate](https://learn.microsoft.com/en-us/dotnet/aspire/troubleshooting/untrusted-localhost-certificate).

## Azure credentials (optional, future use)

MeatyTimes deploys to Azure Container Apps via `aspire deploy`. The AppHost includes `AddAzureContainerAppEnvironment("aca-env")` for deployment metadata. **Current CI tests do not provision or call Azure resources** and do not require Azure authentication.

When a future test or AppHost scenario needs Azure, configure these GitHub Actions secrets:

| Variable | Description |
| --- | --- |
| `AZURE_CLIENT_ID` | Service principal application (client) ID |
| `AZURE_TENANT_ID` | Azure AD tenant ID |
| `AZURE_CLIENT_SECRET` | Service principal client secret |
| `AZURE_SUBSCRIPTION_ID` | Azure subscription ID |

See [Testing in CI/CD pipelines | Aspire](https://aspire.dev/testing/testing-in-ci/) for the standard `DefaultAzureCredential` environment-variable pattern.
