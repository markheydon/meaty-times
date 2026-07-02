using MeatyTimes.Core.Domain;
using MeatyTimes.Core.Rules;

namespace MeatyTimes.Core.Calculation;

/// <summary>
/// Calculates roasting instructions from weight-based rules with optional doneness adjustments.
/// </summary>
public sealed class RoastCalculator
{
    private readonly CookingRuleLoader _ruleLoader;

    public RoastCalculator(CookingRuleLoader ruleLoader)
    {
        _ruleLoader = ruleLoader;
    }

    /// <summary>
    /// Produces deterministic cooking instructions for the given request.
    /// </summary>
    public CookingResult Calculate(RoastRequest request)
    {
        var rule = _ruleLoader.GetRule(request.MeatType);
        ValidateRequest(request, rule);

        // Base cooking time scales with weight (minutes per kg).
        var totalCooking = (int)Math.Round(rule.MinutesPerKg * request.WeightKg, MidpointRounding.AwayFromZero);

        // Doneness shifts total time for beef and lamb only.
        if (rule.SupportsDoneness && request.Doneness is { } doneness)
        {
            var key = doneness.ToString();
            if (rule.DonenessAdjustments?.TryGetValue(key, out var adjustment) == true)
            {
                totalCooking += adjustment;
            }
        }

        totalCooking = Math.Clamp(totalCooking, rule.MinCookingMinutes, rule.MaxCookingMinutes);

        var phases = BuildPhases(rule, totalCooking);

        // Resting scales with weight but is capped to keep guidance practical.
        var resting = (int)Math.Round(rule.RestingMinutesPerKg * request.WeightKg, MidpointRounding.AwayFromZero);
        resting = Math.Clamp(resting, rule.MinRestingMinutes, rule.MaxRestingMinutes);

        return new CookingResult(
            request.MeatType,
            request.WeightKg,
            request.Doneness,
            CookingMethod.TraditionalRoast,
            phases,
            totalCooking,
            resting,
            totalCooking + resting,
            rule.Source);
    }

    private static void ValidateRequest(RoastRequest request, CookingRule rule)
    {
        if (request.WeightKg <= 0)
        {
            throw new RoastValidationException("weightKg", "Enter a weight greater than 0 kg.");
        }

        if (request.WeightKg < rule.MinWeightKg)
        {
            throw new RoastValidationException(
                "weightKg",
                $"Minimum weight for {rule.DisplayName.ToLowerInvariant()} is {rule.MinWeightKg} kg.");
        }

        if (request.WeightKg > rule.MaxWeightKg)
        {
            throw new RoastValidationException(
                "weightKg",
                $"Maximum weight for {rule.DisplayName.ToLowerInvariant()} is {rule.MaxWeightKg} kg.");
        }

        if (rule.SupportsDoneness && request.Doneness is null)
        {
            throw new RoastValidationException("doneness", "Select a doneness level.");
        }
    }

    private static IReadOnlyList<CookingPhase> BuildPhases(CookingRule rule, int totalCookingMinutes)
    {
        // Two-phase roast: brief high heat then reduce (common for beef and lamb).
        if (rule.SecondaryTemperatureC is { } secondaryTemp && rule.InitialPhaseFraction is { } fraction)
        {
            var initialDuration = (int)Math.Round(totalCookingMinutes * fraction, MidpointRounding.AwayFromZero);
            initialDuration = Math.Clamp(initialDuration, 10, totalCookingMinutes - 10);
            var remainingDuration = totalCookingMinutes - initialDuration;

            return
            [
                new CookingPhase(1, "Preheat oven and roast at initial temperature", rule.InitialTemperatureC, initialDuration),
                new CookingPhase(2, "Reduce temperature and continue roasting", secondaryTemp, remainingDuration),
            ];
        }

        // Single-temperature roast (pork, chicken, gammon).
        return
        [
            new CookingPhase(1, "Roast at oven temperature", rule.InitialTemperatureC, totalCookingMinutes),
        ];
    }
}
