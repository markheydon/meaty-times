using MeatyTimes.Core.Domain;

namespace MeatyTimes.Core.Tests;

public class RoastCalculatorOtherMeatsTests
{
    [Fact]
    public void Pork_returns_single_phase_safe_cook_profile()
    {
        var calculator = TestFixtures.CreateCalculator();
        var result = calculator.Calculate(new RoastRequest(MeatTypeId.Pork, 3.0m));

        Assert.Single(result.Phases);
        Assert.Equal(180, result.Phases[0].TemperatureC);
        Assert.Null(result.Doneness);
        Assert.Equal(90, result.TotalCookingMinutes);
    }

    [Fact]
    public void Chicken_returns_single_phase_without_doneness()
    {
        var calculator = TestFixtures.CreateCalculator();
        var result = calculator.Calculate(new RoastRequest(MeatTypeId.Chicken, 1.8m));

        Assert.Single(result.Phases);
        Assert.Equal(180, result.Phases[0].TemperatureC);
        Assert.Null(result.Doneness);
        Assert.True(result.TotalCookingMinutes >= 60);
    }

    [Fact]
    public void Gammon_returns_valid_resting_and_cooking_times()
    {
        var calculator = TestFixtures.CreateCalculator();
        var result = calculator.Calculate(new RoastRequest(MeatTypeId.Gammon, 2.5m));

        Assert.Single(result.Phases);
        Assert.Equal(88, result.TotalCookingMinutes);
        Assert.True(result.RestingMinutes >= 15);
        Assert.False(string.IsNullOrWhiteSpace(result.Source));
    }
}
