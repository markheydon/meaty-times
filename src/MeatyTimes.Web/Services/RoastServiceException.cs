using MeatyTimes.Core.Domain;

namespace MeatyTimes.Web.Services;

/// <summary>
/// Thrown when roast calculation or scheduling fails validation.
/// </summary>
public sealed class RoastServiceException : Exception
{
    public RoastServiceException(string message, Dictionary<string, string[]>? errors = null)
        : base(message)
    {
        Errors = errors ?? new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
    }

    public Dictionary<string, string[]> Errors { get; }

    public string FirstError =>
        Errors.Values.SelectMany(v => v).FirstOrDefault() ?? Message;

    public static RoastServiceException FromValidation(RoastValidationException ex)
    {
        if (ex.Field is { } field)
        {
            return new RoastServiceException(
                ex.Message,
                new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [field] = [ex.Message],
                });
        }

        return new RoastServiceException(ex.Message);
    }
}
