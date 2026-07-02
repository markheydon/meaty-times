namespace MeatyTimes.Core.Domain;

/// <summary>
/// A single step in the roasting process (one temperature for a duration).
/// </summary>
public sealed record CookingPhase(
    int Order,
    string Description,
    int TemperatureC,
    int DurationMinutes);
