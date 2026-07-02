namespace MeatyTimes.Tests;

internal static class IntegrationTestDefaults
{
    // CI runners (GitHub Actions) are slower to start Aspire resources.
    internal static readonly TimeSpan Timeout = TimeSpan.FromSeconds(60);

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
