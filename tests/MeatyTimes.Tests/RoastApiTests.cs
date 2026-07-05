using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace MeatyTimes.Tests;

public class RoastApiTests
{
    private static readonly bool IsRunningInCi = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("CI"));
    private static readonly TimeSpan DefaultTimeout = IsRunningInCi ? TimeSpan.FromMinutes(5) : TimeSpan.FromSeconds(60);

    [Fact]
    public async Task Calculate_beef_returns_ok_with_phases()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.MeatyTimes_AppHost>(cancellationToken);
        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
        });
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        await using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        await app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

        var httpClient = app.CreateHttpClient("apiservice");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("apiservice", cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

        var response = await httpClient.PostAsJsonAsync(
            "/api/roast/calculate",
            new { meatType = "beef", weightKg = 2.0m, doneness = "Medium" },
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        Assert.Contains("phases", json, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("totalCookingMinutes", json, StringComparison.OrdinalIgnoreCase);
    }

}
