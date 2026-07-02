using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace MeatyTimes.Tests;

public class RoastApiTests
{
    [Fact]
    public async Task Calculate_beef_returns_ok_with_phases()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var timeout = IntegrationTestDefaults.Timeout;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.MeatyTimes_AppHost>(cancellationToken);
        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
        });
        IntegrationTestDefaults.ConfigureHttpClients(appHost.Services);

        await using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(timeout, cancellationToken);
        await app.StartAsync(cancellationToken).WaitAsync(timeout, cancellationToken);

        var httpClient = app.CreateHttpClient("apiservice");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("apiservice", cancellationToken).WaitAsync(timeout, cancellationToken);

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
