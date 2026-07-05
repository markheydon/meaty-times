# CI Azure credentials for Aspire tests

The GitHub Actions workflow in [.github/workflows/ci.yml](../.github/workflows/ci.yml) relies on the standard GitHub Actions environment for the test run.

GitHub Actions already sets `CI=true` automatically for workflow jobs, so the workflow does not need to set that variable explicitly.

The Azure-related variables below are only relevant if the Aspire/AppHost path needs to authenticate to Azure resources in a future scenario:

- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_CLIENT_SECRET`
- `AZURE_SUBSCRIPTION_ID`

Their details have been left here for the time being, but they are not required for the current workflow to run the build and test steps. The workflow will still run successfully without them.

## Why these variables exist

MeatyTimes uses Aspire and the AppHost is configured with Azure Container Apps-related behavior. If future tests or AppHost scenarios need to provision or call Azure resources, the Azure SDK can use the standard environment-variable pattern for authentication.

For the current test suite, the main requirement is to make the AppHost integration tests reliable in CI by giving them enough timeout headroom and by letting the standard GitHub Actions environment drive the test behavior.

- `CI=true` is already provided by GitHub Actions.
- The Azure variables are optional for the current repo state.
- They become relevant only when a future test or deployment path needs real Azure authentication.
- The current workflow does not need to inject them just to run the existing build and test steps.
