using MeatyTimes.Core.Domain;

namespace MeatyTimes.Core.Tests;

public class ScheduleCalculatorTests
{
    [Fact]
    public void Achievable_schedule_has_correct_milestone_order()
    {
        var calculator = TestFixtures.CreateScheduleCalculator();
        var now = DateTimeOffset.UtcNow;
        var servingTime = now.AddHours(3);
        var request = new RoastRequest(MeatTypeId.Beef, 2.0m, Doneness.Medium);

        var schedule = calculator.CalculateSchedule(request, servingTime, now);

        Assert.True(schedule.IsAchievable);
        Assert.NotNull(schedule.StartCookingTime);
        Assert.NotNull(schedule.TemperatureChangeTime);
        Assert.NotNull(schedule.RemoveFromOvenTime);
        Assert.NotNull(schedule.RestingStartTime);

        Assert.True(schedule.StartCookingTime < schedule.TemperatureChangeTime);
        Assert.True(schedule.TemperatureChangeTime < schedule.RemoveFromOvenTime);
        Assert.Equal(schedule.RemoveFromOvenTime, schedule.RestingStartTime);
        Assert.Equal(servingTime, schedule.ServingTime);
    }

    [Fact]
    public void Single_phase_roast_omits_temperature_change_milestone()
    {
        var calculator = TestFixtures.CreateScheduleCalculator();
        var now = DateTimeOffset.UtcNow;
        var servingTime = now.AddHours(4);
        var request = new RoastRequest(MeatTypeId.Pork, 2.0m);

        var schedule = calculator.CalculateSchedule(request, servingTime, now);

        Assert.True(schedule.IsAchievable);
        Assert.Null(schedule.TemperatureChangeTime);
        Assert.NotNull(schedule.StartCookingTime);
        Assert.NotNull(schedule.RemoveFromOvenTime);
    }
}
