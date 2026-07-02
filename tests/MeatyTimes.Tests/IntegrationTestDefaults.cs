namespace MeatyTimes.Tests;

internal static class IntegrationTestDefaults
{
    // GitHub Actions Linux runners are slower to cold-start Blazor + API via DCP.
    internal static readonly TimeSpan Timeout = TimeSpan.FromSeconds(
        string.Equals(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"), "true", StringComparison.OrdinalIgnoreCase)
            ? 120
            : 60);

    internal static void ConfigureHttpClients(IServiceCollection services)
    {
        services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            });
            clientBuilder.AddStandardResilienceHandler();
        });
    }
}
