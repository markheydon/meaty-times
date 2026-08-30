namespace MeatyTimes.Web.Services;

public sealed record MeatTypeDto(
    string Id,
    string DisplayName,
    bool SupportsDoneness,
    IReadOnlyList<string> DonenessOptions,
    decimal MinWeightKg,
    decimal MaxWeightKg);

public sealed record PhaseDto(int Order, string Description, int TemperatureC, int DurationMinutes);

public sealed record CookingResultDto(
    string MeatType,
    decimal WeightKg,
    string? Doneness,
    string CookingMethod,
    IReadOnlyList<PhaseDto> Phases,
    int TotalCookingMinutes,
    int RestingMinutes,
    int TotalPreparationMinutes,
    string Source);

public sealed record ScheduleDto(
    DateTimeOffset ServingTime,
    DateTimeOffset? StartCookingTime,
    DateTimeOffset? TemperatureChangeTime,
    DateTimeOffset? RemoveFromOvenTime,
    DateTimeOffset? RestingStartTime,
    bool IsAchievable,
    DateTimeOffset? EarliestServingTime,
    CookingResultDto Instructions);
