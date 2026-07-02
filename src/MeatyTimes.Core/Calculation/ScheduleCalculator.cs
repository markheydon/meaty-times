using MeatyTimes.Core.Domain;

namespace MeatyTimes.Core.Calculation;

/// <summary>
/// Works backwards from a serving time using calculated cooking and resting durations.
/// All comparisons use absolute instants (UTC); returned offsets preserve the serving-time zone.
/// </summary>
public sealed class ScheduleCalculator
{
    private readonly RoastCalculator _roastCalculator;

    public ScheduleCalculator(RoastCalculator roastCalculator)
    {
        _roastCalculator = roastCalculator;
    }

    /// <summary>
    /// Builds a cooking schedule from roast parameters and a target serving time.
    /// </summary>
    public CookingSchedule CalculateSchedule(
        RoastRequest request,
        DateTimeOffset servingTime,
        DateTimeOffset? now = null)
    {
        var currentTime = now ?? DateTimeOffset.UtcNow;

        if (servingTime <= currentTime)
        {
            throw new RoastValidationException("servingTime", "Serving time must be in the future.");
        }

        var instructions = _roastCalculator.Calculate(request);
        var earliestServing = currentTime.AddMinutes(instructions.TotalPreparationMinutes);

        if (servingTime < earliestServing)
        {
            return new CookingSchedule(
                servingTime,
                null,
                null,
                null,
                null,
                false,
                earliestServing,
                instructions);
        }

        var restingStart = servingTime.AddMinutes(-instructions.RestingMinutes);
        var removeFromOven = restingStart;
        var startCooking = removeFromOven.AddMinutes(-instructions.TotalCookingMinutes);

        DateTimeOffset? temperatureChange = null;
        if (instructions.Phases.Count > 1)
        {
            temperatureChange = startCooking.AddMinutes(instructions.Phases[0].DurationMinutes);
        }

        return new CookingSchedule(
            servingTime,
            startCooking,
            temperatureChange,
            removeFromOven,
            restingStart,
            true,
            null,
            instructions);
    }
}
