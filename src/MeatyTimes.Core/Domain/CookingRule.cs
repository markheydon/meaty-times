namespace MeatyTimes.Core.Domain;

/// <summary>
/// JSON-deserializable cooking rule for a meat type and method.
/// </summary>
public sealed class CookingRule
{
    public required string MeatType { get; init; }

    public required string DisplayName { get; init; }

    public bool SupportsDoneness { get; init; }

    public decimal MinWeightKg { get; init; }

    public decimal MaxWeightKg { get; init; }

    public required string CookingMethod { get; init; }

    public required string Source { get; init; }

    public int InitialTemperatureC { get; init; }

    public int? SecondaryTemperatureC { get; init; }

    public decimal MinutesPerKg { get; init; }

    public decimal? InitialPhaseFraction { get; init; }

    public Dictionary<string, int>? DonenessAdjustments { get; init; }

    public decimal RestingMinutesPerKg { get; init; }

    public int MinRestingMinutes { get; init; }

    public int MaxRestingMinutes { get; init; }

    public int MinCookingMinutes { get; init; }

    public int MaxCookingMinutes { get; init; }
}
