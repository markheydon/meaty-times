namespace MeatyTimes.Core.Domain;

/// <summary>
/// User inputs for a roast calculation.
/// </summary>
public sealed record RoastRequest(
    MeatTypeId MeatType,
    decimal WeightKg,
    Doneness? Doneness = null,
    DateTime? ServingTime = null)
{
    /// <summary>
    /// Parses UI/API string inputs into a validated <see cref="RoastRequest"/>.
    /// </summary>
    public static RoastRequest FromInputs(string meatType, decimal weightKg, string? doneness)
    {
        if (!Enum.TryParse<MeatTypeId>(meatType, ignoreCase: true, out var meatTypeId))
        {
            throw new RoastValidationException("meatType", "Unsupported meat type.");
        }

        Doneness? parsedDoneness = null;
        if (!string.IsNullOrWhiteSpace(doneness))
        {
            if (!Enum.TryParse<Doneness>(doneness, ignoreCase: true, out var parsed))
            {
                throw new RoastValidationException("doneness", "Unsupported doneness level.");
            }

            parsedDoneness = parsed;
        }

        return new RoastRequest(meatTypeId, weightKg, parsedDoneness);
    }
}
