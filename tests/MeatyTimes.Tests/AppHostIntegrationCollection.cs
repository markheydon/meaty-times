namespace MeatyTimes.Tests;

/// <summary>
/// Aspire AppHost integration tests must not run in parallel — each test starts DCP
/// and child processes, which conflicts on constrained CI runners when concurrent.
/// import-to-planner avoids this by keeping its AppHost tests in a single test class.
/// </summary>
[CollectionDefinition("AppHost", DisableParallelization = true)]
public sealed class AppHostIntegrationCollection;
