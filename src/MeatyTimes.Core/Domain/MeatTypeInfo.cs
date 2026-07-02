namespace MeatyTimes.Core.Domain;

/// <summary>
/// Metadata about a supported meat type for UI and API listing.
/// </summary>
public sealed record MeatTypeInfo(
    MeatTypeId Id,
    string DisplayName,
    bool SupportsDoneness,
    IReadOnlyList<Doneness> DonenessOptions,
    decimal MinWeightKg,
    decimal MaxWeightKg,
    CookingMethod DefaultCookingMethod);
