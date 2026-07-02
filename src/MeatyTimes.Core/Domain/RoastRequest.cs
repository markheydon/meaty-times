namespace MeatyTimes.Core.Domain;

/// <summary>
/// User inputs for a roast calculation.
/// </summary>
public sealed record RoastRequest(
    MeatTypeId MeatType,
    decimal WeightKg,
    Doneness? Doneness = null,
    DateTime? ServingTime = null);
