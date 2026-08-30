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

    public static int FormatTemperatureFahrenheit(int celsius)
    {
        var fahrenheit = celsius * 9.0 / 5.0 + 32.0;
        return (int)(Math.Round(fahrenheit / 5.0) * 5);
    }

    public static string FormatTemperatureCelsius(int celsius) => $"{celsius} °C";

    public static string FormatTemperatureFahrenheitDisplay(int celsius) =>
        $"{FormatTemperatureFahrenheit(celsius)} °F";

    public static string FormatDurationCompact(int minutes)
    {
        if (minutes < 60)
        {
            return $"{minutes} min";
        }

        var hours = minutes / 60;
        var remaining = minutes % 60;

        if (remaining == 0)
        {
            return hours == 1 ? "1 hr" : $"{hours} hr";
        }

        var hourPart = hours == 1 ? "1 hr" : $"{hours} hr";
        return $"{hourPart} {remaining} min";
    }
}
