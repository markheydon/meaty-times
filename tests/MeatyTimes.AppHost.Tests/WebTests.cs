namespace MeatyTimes.AppHost.Tests;

[Collection(AspireTestCollection.Name)]
public class WebTests(AspireAppHostFixture fixture)
{
    [Fact]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var httpClient = fixture.App.CreateHttpClient("webfrontend");
        var response = await httpClient.GetAsync("/", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
