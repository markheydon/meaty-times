using Bunit;
using MeatyTimes.Web.Components.Roast;
using MeatyTimes.Web.Services;

namespace MeatyTimes.Web.Tests;

public class ServeAtPlannerTests : BunitContext
{
    public ServeAtPlannerTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Renders_nothing_when_schedule_and_error_are_absent()
    {
        var cut = Render<ServeAtPlanner>();

        Assert.Empty(cut.Markup.Trim());
    }

    [Fact]
    public void Renders_achievable_schedule_milestones()
    {
        var schedule = CreateAchievableSchedule();

        var cut = Render<ServeAtPlanner>(parameters => parameters
            .Add(p => p.Schedule, schedule));

        Assert.Contains("Schedule", cut.Markup);
        Assert.Contains("Start cooking", cut.Markup);
        Assert.Contains("Remove from oven", cut.Markup);
        Assert.Contains("Resting begins", cut.Markup);
    }

    [Fact]
    public void Renders_temperature_change_milestone_when_present()
    {
        var baseSchedule = CreateAchievableSchedule();
        var schedule = baseSchedule with
        {
            TemperatureChangeTime = baseSchedule.StartCookingTime!.Value.AddMinutes(30),
        };

        var cut = Render<ServeAtPlanner>(parameters => parameters
            .Add(p => p.Schedule, schedule));

        Assert.Contains("Reduce temperature", cut.Markup);
    }

    [Fact]
    public void Renders_unachievable_warning_with_earliest_serving_time()
    {
        var earliest = DateTimeOffset.UtcNow.AddHours(2);
        var schedule = CreateAchievableSchedule() with
        {
            IsAchievable = false,
            EarliestServingTime = earliest,
        };

        var cut = Render<ServeAtPlanner>(parameters => parameters
            .Add(p => p.Schedule, schedule));

        Assert.Contains("too soon", cut.Markup, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Earliest possible", cut.Markup);
        Assert.Contains(earliest.ToLocalTime().ToString("HH:mm"), cut.Markup);
    }

    [Fact]
    public void Renders_schedule_error_message()
    {
        const string error = "Serving time must be in the future.";

        var cut = Render<ServeAtPlanner>(parameters => parameters
            .Add(p => p.Error, error));

        Assert.Contains(error, cut.Markup);
        Assert.Contains("role=\"alert\"", cut.Markup);
    }

    private static ScheduleDto CreateAchievableSchedule()
    {
        var servingTime = DateTimeOffset.UtcNow.AddHours(4);
        var instructions = new CookingResultDto(
            "beef",
            2.0m,
            "Medium",
            "TraditionalRoast",
            [new(1, "Roast", 180, 60)],
            60,
            20,
            80,
            "Test source");

        return new ScheduleDto(
            servingTime,
            servingTime.AddHours(-2),
            null,
            servingTime.AddMinutes(-20),
            servingTime.AddMinutes(-20),
            true,
            null,
            instructions);
    }
}
