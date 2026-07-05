using Aspire.Hosting;
using Microsoft.Extensions.Logging;

namespace MeatyTimes.AppHost.Tests;

public sealed class AspireAppHostFixture : IAsyncLifetime, IAsyncDisposable
{
    private static readonly TimeSpan DefaultTimeout =
        !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("CI"))
            ? TimeSpan.FromMinutes(2)
            : TimeSpan.FromSeconds(60);

    private DistributedApplication? _app;

    public DistributedApplication App =>
        _app ?? throw new InvalidOperationException("The AppHost has not been started.");

    public async ValueTask InitializeAsync()
    {
        using var cts = new CancellationTokenSource(DefaultTimeout);
        var cancellationToken = cts.Token;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.MeatyTimes_AppHost>(
            ["--environment=Development"],
            cancellationToken);

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

        var app = await appHost.BuildAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        await app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

        await app.ResourceNotifications.WaitForResourceHealthyAsync("apiservice", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        await app.ResourceNotifications.WaitForResourceHealthyAsync("webfrontend", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        _app = app;
    }

    public async ValueTask DisposeAsync()
    {
        if (_app is not null)
        {
            await _app.DisposeAsync();
            _app = null;
        }
    }
}
