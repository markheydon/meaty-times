using Microsoft.Extensions.Logging;

namespace MeatyTimes.Tests;

public class WebTests
{
    [Fact]
    public async Task GetWebHealthEndpointReturnsOkStatusCode()
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

        var httpClient = app.CreateHttpClient("webfrontend");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("webfrontend", cancellationToken).WaitAsync(timeout, cancellationToken);
        var response = await httpClient.GetAsync("/health", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
