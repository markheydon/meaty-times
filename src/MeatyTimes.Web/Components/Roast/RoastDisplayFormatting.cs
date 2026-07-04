namespace MeatyTimes.Web.Components.Roast;

/// <summary>
/// Shared user-facing labels for roast input fields shown in the form and results summary.
/// </summary>
public static class RoastDisplayFormatting
{
    public static string FormatDoneness(string value) => value switch
    {
        "WellDone" => "Well Done",
        _ => value,
    };

    public static string FormatWeightKg(decimal weightKg) => $"{weightKg:0.0} kg";
}
