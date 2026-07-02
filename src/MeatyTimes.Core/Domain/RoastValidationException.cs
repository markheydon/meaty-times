namespace MeatyTimes.Core.Domain;

/// <summary>
/// Thrown when roast inputs fail validation. Message is safe to show home cooks.
/// </summary>
public sealed class RoastValidationException : Exception
{
    public RoastValidationException(string message)
        : base(message)
    {
    }

    public RoastValidationException(string field, string message)
        : base(message)
    {
        Field = field;
    }

    public string? Field { get; }
}
