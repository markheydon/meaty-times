using System.Net.Http.Json;

namespace MeatyTimes.AppHost.Tests;

[Collection(AspireTestCollection.Name)]
public class RoastApiTests(AspireAppHostFixture fixture)
{
    [Fact]
    public async Task Calculate_beef_returns_ok_with_phases()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var httpClient = fixture.App.CreateHttpClient("apiservice");

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
