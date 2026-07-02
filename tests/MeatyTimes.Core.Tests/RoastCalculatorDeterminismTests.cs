using MeatyTimes.Core.Domain;

namespace MeatyTimes.Core.Tests;

public class RoastCalculatorDeterminismTests
{
    [Fact]
    public void Same_inputs_produce_same_outputs()
    {
        var calculator = TestFixtures.CreateCalculator();
        var request = new RoastRequest(MeatTypeId.Beef, 2.0m, Doneness.Medium);

        var first = calculator.Calculate(request);
        var second = calculator.Calculate(request);
        var third = calculator.Calculate(request);

        Assert.Equal(first.TotalCookingMinutes, second.TotalCookingMinutes);
        Assert.Equal(second.TotalCookingMinutes, third.TotalCookingMinutes);
        Assert.Equal(first.RestingMinutes, second.RestingMinutes);
        Assert.Equal(first.Phases.Count, third.Phases.Count);
    }
}
