using MeatyTimes.Core.Domain;

namespace MeatyTimes.Core.Tests;

public class ScheduleCalculatorUnachievableTests
{
    [Fact]
    public void Serving_time_too_soon_is_not_achievable()
    {
        var calculator = TestFixtures.CreateScheduleCalculator();
        var now = new DateTimeOffset(2026, 7, 5, 12, 0, 0, TimeSpan.Zero);
        var servingTime = now.AddMinutes(30);
        var request = new RoastRequest(MeatTypeId.Beef, 2.0m, Doneness.Medium);

        var schedule = calculator.CalculateSchedule(request, servingTime, now);

        Assert.False(schedule.IsAchievable);
        Assert.Null(schedule.StartCookingTime);
        Assert.NotNull(schedule.EarliestServingTime);
        Assert.True(schedule.EarliestServingTime > servingTime);
    }

    [Fact]
    public void Serving_time_in_past_throws_validation_error()
    {
        var calculator = TestFixtures.CreateScheduleCalculator();
        var request = new RoastRequest(MeatTypeId.Beef, 2.0m, Doneness.Medium);
        var now = DateTimeOffset.UtcNow;

        var ex = Assert.Throws<RoastValidationException>(() =>
            calculator.CalculateSchedule(request, now.AddMinutes(-5), now));

        Assert.Equal("servingTime", ex.Field);
    }
}
