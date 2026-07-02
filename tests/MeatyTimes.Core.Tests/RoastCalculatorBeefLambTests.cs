using MeatyTimes.Core.Domain;

namespace MeatyTimes.Core.Tests;

public class RoastCalculatorBeefLambTests
{
    [Fact]
    public void Returns_two_phase_profile_for_beef_at_medium()
    {
        var calculator = TestFixtures.CreateCalculator();
        var result = calculator.Calculate(new RoastRequest(MeatTypeId.Beef, 2.0m, Doneness.Medium));

        Assert.Equal(2, result.Phases.Count);
        Assert.Equal(220, result.Phases[0].TemperatureC);
        Assert.Equal(160, result.Phases[1].TemperatureC);
        Assert.Equal(40, result.TotalCookingMinutes);
        Assert.True(result.RestingMinutes >= 15);
        Assert.Equal(result.TotalCookingMinutes + result.RestingMinutes, result.TotalPreparationMinutes);
    }

    [Fact]
    public void Rare_beef_cooks_shorter_than_well_done()
    {
        var calculator = TestFixtures.CreateCalculator();

        var rare = calculator.Calculate(new RoastRequest(MeatTypeId.Beef, 2.0m, Doneness.Rare));
        var wellDone = calculator.Calculate(new RoastRequest(MeatTypeId.Beef, 2.0m, Doneness.WellDone));

        Assert.True(rare.TotalCookingMinutes < wellDone.TotalCookingMinutes);
    }

    [Fact]
    public void Returns_two_phase_profile_for_lamb_at_medium()
    {
        var calculator = TestFixtures.CreateCalculator();
        var result = calculator.Calculate(new RoastRequest(MeatTypeId.Lamb, 1.5m, Doneness.Medium));

        Assert.Equal(2, result.Phases.Count);
        Assert.Equal(220, result.Phases[0].TemperatureC);
        Assert.Equal(180, result.Phases[1].TemperatureC);
        Assert.True(result.TotalCookingMinutes >= 35);
    }

    [Fact]
    public void Rare_lamb_cooks_shorter_than_well_done()
    {
        var calculator = TestFixtures.CreateCalculator();

        var rare = calculator.Calculate(new RoastRequest(MeatTypeId.Lamb, 2.0m, Doneness.Rare));
        var wellDone = calculator.Calculate(new RoastRequest(MeatTypeId.Lamb, 2.0m, Doneness.WellDone));

        Assert.True(rare.TotalCookingMinutes < wellDone.TotalCookingMinutes);
    }
}
