namespace MeatyTimes.Core.Domain;

/// <summary>
/// Calculated roasting instructions shown to the user.
/// </summary>
public sealed record CookingResult(
    MeatTypeId MeatType,
    decimal WeightKg,
    Doneness? Doneness,
    CookingMethod CookingMethod,
    IReadOnlyList<CookingPhase> Phases,
    int TotalCookingMinutes,
    int RestingMinutes,
    int TotalPreparationMinutes,
    string Source);
