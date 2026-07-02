using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Keep child service environments aligned with the AppHost so Aspire health checks
// and integration tests work outside local Development runs (e.g. Release CI builds).
var appRuntimeEnvironment = builder.Environment.IsProduction()
    ? "Production"
    : builder.Environment.IsStaging()
        ? "Staging"
        : "Development";

var apiService = builder.AddProject<Projects.MeatyTimes_ApiService>("apiservice")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", appRuntimeEnvironment)
    .WithEnvironment("DOTNET_ENVIRONMENT", appRuntimeEnvironment)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.MeatyTimes_Web>("webfrontend")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", appRuntimeEnvironment)
    .WithEnvironment("DOTNET_ENVIRONMENT", appRuntimeEnvironment)
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
