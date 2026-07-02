using System.Net;
using System.Text.Json;

namespace MeatyTimes.Web.Services;

public sealed class RoastApiClient(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public async Task<IReadOnlyList<MeatTypeDto>> GetMeatsAsync(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response;
        try
        {
            response = await httpClient.GetAsync("/api/meats", cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            throw RoastApiException.TransportFailure(ex);
        }

        await EnsureSuccessAsync(response, cancellationToken);

        var payload = await response.Content.ReadFromJsonAsync<MeatsResponse>(JsonOptions, cancellationToken);
        return payload?.Meats ?? [];
    }

    public async Task<CookingResultDto> CalculateAsync(
        string meatType,
        decimal weightKg,
        string? doneness,
        CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response;
        try
        {
            response = await httpClient.PostAsJsonAsync(
                "/api/roast/calculate",
                new CalculateRequestDto(meatType, weightKg, doneness),
                cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            throw RoastApiException.TransportFailure(ex);
        }

        await EnsureSuccessAsync(response, cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<CookingResultDto>(JsonOptions, cancellationToken);
        return result ?? throw RoastApiException.UnexpectedResponse((int)response.StatusCode);
    }

    public async Task<ScheduleDto> PlanScheduleAsync(
        string meatType,
        decimal weightKg,
        string? doneness,
        DateTimeOffset servingTime,
        CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response;
        try
        {
            response = await httpClient.PostAsJsonAsync(
                "/api/roast/schedule",
                new ScheduleRequestDto(meatType, weightKg, doneness, servingTime),
                cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            throw RoastApiException.TransportFailure(ex);
        }

        await EnsureSuccessAsync(response, cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<ScheduleDto>(JsonOptions, cancellationToken);
        return result ?? throw RoastApiException.UnexpectedResponse((int)response.StatusCode);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var statusCode = (int)response.StatusCode;
        var body = response.Content is null
            ? string.Empty
            : await response.Content.ReadAsStringAsync(cancellationToken);

        var errors = TryParseValidationErrors(body);
        var detail = TryParseProblemDetail(body);

        throw new RoastApiException(
            statusCode,
            errors,
            detail ?? $"The server returned status {statusCode} ({response.StatusCode}).");
    }

    private static Dictionary<string, string[]>? TryParseValidationErrors(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(body);
            if (!document.RootElement.TryGetProperty("errors", out var errorsElement)
                || errorsElement.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            var errors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            foreach (var property in errorsElement.EnumerateObject())
            {
                if (property.Value.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                var messages = property.Value.EnumerateArray()
                    .Where(item => item.ValueKind == JsonValueKind.String)
                    .Select(item => item.GetString())
                    .Where(message => !string.IsNullOrWhiteSpace(message))
                    .Select(message => message!)
                    .ToArray();

                if (messages.Length > 0)
                {
                    errors[property.Name] = messages;
                }
            }

            return errors.Count > 0 ? errors : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string? TryParseProblemDetail(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(body);
            var root = document.RootElement;

            if (root.TryGetProperty("detail", out var detail) && detail.ValueKind == JsonValueKind.String)
            {
                var detailText = detail.GetString();
                if (!string.IsNullOrWhiteSpace(detailText))
                {
                    return detailText;
                }
            }

            if (root.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String)
            {
                return title.GetString();
            }

            return null;
        }
        catch (JsonException)
        {
            var trimmed = body.Trim();
            return trimmed.Length <= 200 ? trimmed : null;
        }
    }

    public sealed record MeatsResponse(IReadOnlyList<MeatTypeDto> Meats);

    public sealed record MeatTypeDto(
        string Id,
        string DisplayName,
        bool SupportsDoneness,
        IReadOnlyList<string> DonenessOptions,
        decimal MinWeightKg,
        decimal MaxWeightKg);

    public sealed record CalculateRequestDto(string MeatType, decimal WeightKg, string? Doneness);

    public sealed record ScheduleRequestDto(string MeatType, decimal WeightKg, string? Doneness, DateTimeOffset ServingTime);

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
}

public sealed class RoastApiException : Exception
{
    public RoastApiException(int statusCode, Dictionary<string, string[]>? errors, string message)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors ?? new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
    }

    public int StatusCode { get; }

    public Dictionary<string, string[]> Errors { get; }

    public string FirstError =>
        Errors.Values.SelectMany(v => v).FirstOrDefault() ?? Message;

    public static RoastApiException TransportFailure(HttpRequestException ex) =>
        new((int)HttpStatusCode.ServiceUnavailable, null, "Could not reach the server. Check your connection and try again.");

    public static RoastApiException UnexpectedResponse(int statusCode) =>
        new(statusCode, null, $"The server returned an unexpected empty response (status {statusCode}).");
}
