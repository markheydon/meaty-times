# Deploying MeatyTimes to Azure

MeatyTimes deploys to **Azure Container Apps** via **Aspire** (`aspire deploy` from `src/MeatyTimes.AppHost`). The AppHost is the single source of truth for local orchestration and production deployment.

Scale-to-zero is enabled (`MinReplicas = 0`) to minimise idle hosting costs. Expect cold starts and Blazor Server SignalR reconnects after idle periods.

## CI and CD

| Workflow | Trigger | Purpose |
| --- | --- | --- |
| [CI](../.github/workflows/ci.yml) | PR and push to `main` | Format, build, test |
| [Aspire deploy validate](../.github/workflows/aspire-deploy-validate.yml) | PR and push to `main` | `aspire deploy --list-steps` (no Azure auth) |
| [CD](../.github/workflows/cd.yml) | Push to `main`, manual dispatch | Build, test, then `aspire deploy --environment Production` |

CI does not provision Azure resources. CD uses OIDC via the GitHub `production` environment.

## Prerequisites

| Requirement | Notes |
| --- | --- |
| Azure subscription | Permission to create resources in a resource group |
| [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli) | For local `az login` and one-time OIDC setup |
| GitHub repository admin access | To configure the `production` environment and secrets |

## One-time Azure setup (GitHub Actions CD)

Aspire does **not** create the federated credential for GitHub Actions. Run these commands once per subscription and repository.

Set variables for your environment:

```bash
RESOURCE_GROUP="<your-app-resource-group>"
LOCATION="ukwest"
GITHUB_ORG="markheydon"
GITHUB_REPO="meaty-times"
IDENTITY_NAME="id-meatytimes-cd"
```

Create a user-assigned managed identity in the app resource group:

```bash
az identity create \
  --name "$IDENTITY_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --location "$LOCATION"
```

Record the `clientId`, `principalId`, and `id` from the output.

Assign **Contributor** on the resource group:

```bash
PRINCIPAL_ID="$(az identity show --name "$IDENTITY_NAME" --resource-group "$RESOURCE_GROUP" --query principalId -o tsv)"

az role assignment create \
  --assignee-object-id "$PRINCIPAL_ID" \
  --assignee-principal-type ServicePrincipal \
  --role Contributor \
  --scope "/subscriptions/$(az account show --query id -o tsv)/resourceGroups/$RESOURCE_GROUP"
```

Aspire deploy also creates managed-identity role assignments (for example Container Registry and Key Vault access). Grant **User Access Administrator** on the same resource group scope so the CD identity can create those assignments:

```bash
az role assignment create \
  --assignee-object-id "$PRINCIPAL_ID" \
  --assignee-principal-type ServicePrincipal \
  --role "User Access Administrator" \
  --scope "/subscriptions/$(az account show --query id -o tsv)/resourceGroups/$RESOURCE_GROUP"
```

Create a federated credential for the GitHub `production` environment:

```bash
CLIENT_ID="$(az identity show --name "$IDENTITY_NAME" --resource-group "$RESOURCE_GROUP" --query clientId -o tsv)"

az identity federated-credential create \
  --name "github-production" \
  --identity-name "$IDENTITY_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --issuer "https://token.actions.githubusercontent.com" \
  --subject "repo:${GITHUB_ORG}/${GITHUB_REPO}:environment:production" \
  --audiences "api://AzureADTokenExchange"
```

## Configure GitHub Environment

Create a GitHub Environment named `production` in **Settings → Environments**.

**Secrets**

| Secret | Value |
| --- | --- |
| `AZURE_CLIENT_ID` | Managed identity `clientId` from the setup above |
| `AZURE_TENANT_ID` | `az account show --query tenantId -o tsv` |
| `AZURE_SUBSCRIPTION_ID` | `az account show --query id -o tsv` |

**Variables**

| Variable | Example | Purpose |
| --- | --- | --- |
| `AZURE_LOCATION` | `ukwest` | Azure region for `aspire deploy` |
| `AZURE_RESOURCE_GROUP` | `<your-app-resource-group>` | Resource group for the live Container App |

Set `AZURE_LOCATION` and `AZURE_RESOURCE_GROUP` to the **existing** resource group used for manual deploys so CD updates the live site instead of provisioning a second stack.

Optional: enable required reviewers on `production` before granting deploy access.

## Deploy from GitHub Actions

| Trigger | How |
| --- | --- |
| **Production** | Merge to `main`, or **Actions → CD - Deploy to Azure → Run workflow** |

After a successful deploy:

1. Note the Container App FQDN from the workflow output or Azure portal.
2. Smoke-check: `curl -sf "https://<fqdn>/health"`. Cold start after scale-to-zero is expected.

## Deploy locally

Local deploys use Azure CLI credentials (`az login`), not OIDC:

```bash
az login

export Azure__SubscriptionId="$(az account show --query id -o tsv)"
export Azure__Location="ukwest"
export Azure__ResourceGroup="<your-app-resource-group>"

dotnet build MeatyTimes.slnx --configuration Release

aspire deploy --list-steps \
  --apphost src/MeatyTimes.AppHost/MeatyTimes.AppHost.csproj \
  --environment Production \
  --non-interactive

aspire deploy \
  --apphost src/MeatyTimes.AppHost/MeatyTimes.AppHost.csproj \
  --environment Production \
  --non-interactive
```

On Windows PowerShell, use `$env:Azure__SubscriptionId = ...` (and the same pattern for the other variables) instead of `export`.

## HTTPS development certificate (local only)

Aspire AppHost health-checks `webfrontend` over HTTPS during local development. Without a trusted ASP.NET Core development certificate, the web app may never report healthy and AppHost startup can hang.

For local `dotnet run --project src/MeatyTimes.AppHost`, trust the dev certificate:

```powershell
dotnet dev-certs https --trust
```

See [AGENTS.md](../AGENTS.md) and [Aspire troubleshooting: untrusted localhost certificate](https://learn.microsoft.com/en-us/dotnet/aspire/troubleshooting/untrusted-localhost-certificate).

## Resources provisioned by Aspire

A typical deployment includes:

| Resource | Purpose |
| --- | --- |
| Azure Container Apps environment | Hosts the containerised app (Consumption profile) |
| Container App (`webfrontend`) | Runs MeatyTimes (scale-to-zero enabled) |
| Azure Container Registry | Stores built container images |
| Aspire dashboard | Optional operational dashboard (Aspire default) |
| Managed identities | Image pull and runtime authentication |

## Teardown

To remove Aspire-managed resources:

```bash
aspire destroy \
  --apphost src/MeatyTimes.AppHost/MeatyTimes.AppHost.csproj \
  --environment Production \
  --yes \
  --non-interactive
```

Confirm the subscription, resource group, and environment before running destroy. The OIDC managed identity is not removed by `aspire destroy`.

## Troubleshooting

| Symptom | Likely cause | Action |
| --- | --- | --- |
| OIDC login fails in CD | Stale secrets or federated credential subject mismatch | Confirm secrets match the CD managed identity and that a federated credential exists for `repo:markheydon/meaty-times:environment:production` |
| Deploy fails after merge | Missing GitHub Environment variables | Set `AZURE_LOCATION` and `AZURE_RESOURCE_GROUP` on `production` |
| Cold start / SignalR disconnect | Scale-to-zero idle | Expected; refresh the page or wait for the container to warm up |
| `aspire deploy --list-steps` fails in CI | AppHost or hosting package breakage | Fix before merge; CI does not use Azure auth |
