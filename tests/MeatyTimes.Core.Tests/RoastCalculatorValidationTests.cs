using MeatyTimes.Core.Domain;

namespace MeatyTimes.Core.Tests;

public class RoastCalculatorValidationTests
{
    [Fact]
    public void Weight_below_minimum_throws_validation_error()
    {
        var calculator = TestFixtures.CreateCalculator();

        var ex = Assert.Throws<RoastValidationException>(() =>
            calculator.Calculate(new RoastRequest(MeatTypeId.Beef, 0.1m, Doneness.Medium)));

        Assert.Equal("weightKg", ex.Field);
        Assert.Contains("Minimum weight", ex.Message);
    }

    [Fact]
    public void Weight_above_maximum_throws_validation_error()
    {
        var calculator = TestFixtures.CreateCalculator();

        var ex = Assert.Throws<RoastValidationException>(() =>
            calculator.Calculate(new RoastRequest(MeatTypeId.Lamb, 20m, Doneness.Medium)));

        Assert.Equal("weightKg", ex.Field);
        Assert.Contains("Maximum weight", ex.Message);
    }

    [Fact]
    public void Missing_doneness_for_beef_throws_validation_error()
    {
        var calculator = TestFixtures.CreateCalculator();

        var ex = Assert.Throws<RoastValidationException>(() =>
            calculator.Calculate(new RoastRequest(MeatTypeId.Beef, 2.0m)));

        Assert.Equal("doneness", ex.Field);
    }

    [Fact]
    public void Zero_weight_throws_validation_error()
    {
        var calculator = TestFixtures.CreateCalculator();

        var ex = Assert.Throws<RoastValidationException>(() =>
            calculator.Calculate(new RoastRequest(MeatTypeId.Pork, 0m)));

        Assert.Equal("weightKg", ex.Field);
    }
}
