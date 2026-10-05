var builder = DistributedApplication.CreateBuilder(args);

// Add the Azure Container Apps environment to the builder.
builder.AddAzureContainerAppEnvironment("aca-env");

// Add the web frontend project to the builder and configure it to be published as an Azure Container App
// with scaling settings and a health check endpoint.
#pragma warning disable ASPIREDOTNETPROJECT001
var webfrontend = builder.AddDotnetProject("webfrontend", "../MeatyTimes.Web/MeatyTimes.Web.csproj");
#pragma warning restore ASPIREDOTNETPROJECT001
webfrontend
    .PublishAsAzureContainerApp((infra, app) =>
    {
        app.Template.Scale.MinReplicas = 0;
        app.Template.Scale.MaxReplicas = 1;
    })
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health");

// Build and run the distributed application.
builder.Build().Run();
