namespace MeatyTimes.AppHost.Tests;

[CollectionDefinition(Name, DisableParallelization = true)]
public class AspireTestCollection : ICollectionFixture<AspireAppHostFixture>
{
    public const string Name = "Aspire";
}
