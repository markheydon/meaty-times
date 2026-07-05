var builder = DistributedApplication.CreateBuilder(args);

// Add the Azure Container Apps environment to the builder.
builder.AddAzureContainerAppEnvironment("aca-env");

// Add the API service project to the builder and configure it to be published as an Azure Container App
// with scaling settings and a health check endpoint.
var apiService = builder.AddProject<Projects.MeatyTimes_ApiService>("apiservice")
    .PublishAsAzureContainerApp((infra, app) =>
    {
        app.Template.Scale.MinReplicas = 0;
        app.Template.Scale.MaxReplicas = 1;
    })
    .WithHttpHealthCheck("/health");

// Add the web frontend project to the builder and configure it to be published as an Azure Container App
// with scaling settings, a health check endpoint, and a reference to the API service.
builder.AddProject<Projects.MeatyTimes_Web>("webfrontend")
    .PublishAsAzureContainerApp((infra, app) =>
    {
        app.Template.Scale.MinReplicas = 0;
        app.Template.Scale.MaxReplicas = 1;
    })
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

// Build and run the distributed application.
builder.Build().Run();
