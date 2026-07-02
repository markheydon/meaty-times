using MeatyTimes.Core.Calculation;
using MeatyTimes.Core.Domain;
using MeatyTimes.Core.Rules;

namespace MeatyTimes.ApiService.Endpoints;

/// <summary>
/// Minimal API endpoints for roast calculation.
/// </summary>
public static class RoastEndpoints
{
    public static RouteGroupBuilder MapRoastEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api");

        group.MapGet("/meats", (CookingRuleLoader loader) =>
        {
            var meats = loader.GetMeatTypes().Select(m => new MeatTypeResponse(
                m.Id.ToString().ToLowerInvariant(),
                m.DisplayName,
                m.SupportsDoneness,
                m.DonenessOptions.Select(d => d.ToString()).ToList(),
                m.MinWeightKg,
                m.MaxWeightKg));
            return Results.Ok(new MeatsListResponse(meats));
        })
        .WithName("GetMeats");

        group.MapPost("/roast/calculate", (
            CalculateRequest request,
            RoastCalculator calculator) =>
        {
            try
            {
                var roastRequest = ToRoastRequest(request);
                var result = calculator.Calculate(roastRequest);
                return Results.Ok(ToCookingResultResponse(result));
            }
            catch (RoastValidationException ex)
            {
                return ValidationProblem(ex);
            }
        })
        .WithName("CalculateRoast");

        group.MapPost("/roast/schedule", (
            ScheduleRequest request,
            ScheduleCalculator calculator) =>
        {
            try
            {
                var roastRequest = ToRoastRequest(request.MeatType, request.WeightKg, request.Doneness);
                var schedule = calculator.CalculateSchedule(roastRequest, request.ServingTime);
                return Results.Ok(ToScheduleResponse(schedule));
            }
            catch (RoastValidationException ex)
            {
                return ValidationProblem(ex);
            }
        })
        .WithName("ScheduleRoast");

        return group;
    }

    private static RoastRequest ToRoastRequest(CalculateRequest request) =>
        ToRoastRequest(request.MeatType, request.WeightKg, request.Doneness);

    private static RoastRequest ToRoastRequest(string meatType, decimal weightKg, string? doneness)
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

    private static IResult ValidationProblem(RoastValidationException ex)
    {
        if (ex.Field is { } field)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [field] = [ex.Message],
            });
        }

        return Results.BadRequest(new { title = "Validation failed", detail = ex.Message });
    }

    private static CookingResultResponse ToCookingResultResponse(CookingResult result) =>
        new(
            result.MeatType.ToString().ToLowerInvariant(),
            result.WeightKg,
            result.Doneness?.ToString(),
            result.CookingMethod.ToString(),
            result.Phases.Select(p => new PhaseResponse(p.Order, p.Description, p.TemperatureC, p.DurationMinutes)).ToList(),
            result.TotalCookingMinutes,
            result.RestingMinutes,
            result.TotalPreparationMinutes,
            result.Source);

    private static ScheduleResponse ToScheduleResponse(CookingSchedule schedule) =>
        new(
            schedule.ServingTime,
            schedule.StartCookingTime,
            schedule.TemperatureChangeTime,
            schedule.RemoveFromOvenTime,
            schedule.RestingStartTime,
            schedule.IsAchievable,
            schedule.EarliestServingTime,
            ToCookingResultResponse(schedule.Instructions));

    public sealed record CalculateRequest(string MeatType, decimal WeightKg, string? Doneness);

    public sealed record ScheduleRequest(string MeatType, decimal WeightKg, string? Doneness, DateTimeOffset ServingTime);

    public sealed record MeatsListResponse(IEnumerable<MeatTypeResponse> Meats);

    public sealed record MeatTypeResponse(
        string Id,
        string DisplayName,
        bool SupportsDoneness,
        IReadOnlyList<string> DonenessOptions,
        decimal MinWeightKg,
        decimal MaxWeightKg);

    public sealed record PhaseResponse(int Order, string Description, int TemperatureC, int DurationMinutes);

    public sealed record CookingResultResponse(
        string MeatType,
        decimal WeightKg,
        string? Doneness,
        string CookingMethod,
        IReadOnlyList<PhaseResponse> Phases,
        int TotalCookingMinutes,
        int RestingMinutes,
        int TotalPreparationMinutes,
        string Source);

    public sealed record ScheduleResponse(
        DateTimeOffset ServingTime,
        DateTimeOffset? StartCookingTime,
        DateTimeOffset? TemperatureChangeTime,
        DateTimeOffset? RemoveFromOvenTime,
        DateTimeOffset? RestingStartTime,
        bool IsAchievable,
        DateTimeOffset? EarliestServingTime,
        CookingResultResponse Instructions);
}
