using MeatyTimes.Core.Calculation;
using MeatyTimes.Core.Domain;
using MeatyTimes.Core.Rules;

namespace MeatyTimes.Web.Services;

/// <summary>
/// In-process facade over MeatyTimes.Core for the roast calculator UI.
/// </summary>
public sealed class RoastService(
    CookingRuleLoader ruleLoader,
    RoastCalculator calculator,
    ScheduleCalculator scheduleCalculator)
{
    public IReadOnlyList<MeatTypeDto> GetMeats()
    {
        return ruleLoader.GetMeatTypes()
            .Select(m => new MeatTypeDto(
                m.Id.ToString().ToLowerInvariant(),
                m.DisplayName,
                m.SupportsDoneness,
                m.DonenessOptions.Select(d => d.ToString()).ToList(),
                m.MinWeightKg,
                m.MaxWeightKg))
            .ToList();
    }

    public CookingResultDto Calculate(string meatType, decimal weightKg, string? doneness)
    {
        try
        {
            var request = RoastRequest.FromInputs(meatType, weightKg, doneness);
            return ToCookingResultDto(calculator.Calculate(request));
        }
        catch (RoastValidationException ex)
        {
            throw RoastServiceException.FromValidation(ex);
        }
    }

    public ScheduleDto PlanSchedule(
        string meatType,
        decimal weightKg,
        string? doneness,
        DateTimeOffset servingTime)
    {
        try
        {
            var request = RoastRequest.FromInputs(meatType, weightKg, doneness);
            var schedule = scheduleCalculator.CalculateSchedule(request, servingTime);
            return ToScheduleDto(schedule);
        }
        catch (RoastValidationException ex)
        {
            throw RoastServiceException.FromValidation(ex);
        }
    }

    private static CookingResultDto ToCookingResultDto(CookingResult result) =>
        new(
            result.MeatType.ToString().ToLowerInvariant(),
            result.WeightKg,
            result.Doneness?.ToString(),
            result.CookingMethod.ToString(),
            result.Phases.Select(p => new PhaseDto(p.Order, p.Description, p.TemperatureC, p.DurationMinutes)).ToList(),
            result.TotalCookingMinutes,
            result.RestingMinutes,
            result.TotalPreparationMinutes,
            result.Source);

    private static ScheduleDto ToScheduleDto(CookingSchedule schedule) =>
        new(
            schedule.ServingTime,
            schedule.StartCookingTime,
            schedule.TemperatureChangeTime,
            schedule.RemoveFromOvenTime,
            schedule.RestingStartTime,
            schedule.IsAchievable,
            schedule.EarliestServingTime,
            ToCookingResultDto(schedule.Instructions));
}
