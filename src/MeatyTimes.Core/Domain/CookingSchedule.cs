namespace MeatyTimes.Core.Domain;

/// <summary>
/// Backwards-calculated timeline from a target serving time (UTC-based instants).
/// </summary>
public sealed record CookingSchedule(
    DateTimeOffset ServingTime,
    DateTimeOffset? StartCookingTime,
    DateTimeOffset? TemperatureChangeTime,
    DateTimeOffset? RemoveFromOvenTime,
    DateTimeOffset? RestingStartTime,
    bool IsAchievable,
    DateTimeOffset? EarliestServingTime,
    CookingResult Instructions);
