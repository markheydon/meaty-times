using MeatyTimes.Web.Components.Roast;

namespace MeatyTimes.Web.Tests;

public class RoastDisplayFormattingTests
{
    [Theory]
    [InlineData(180, 355)]
    [InlineData(200, 390)]
    [InlineData(100, 210)]
    [InlineData(0, 30)]
    public void FormatTemperatureFahrenheit_rounds_to_nearest_five(int celsius, int expectedFahrenheit)
    {
        Assert.Equal(expectedFahrenheit, RoastDisplayFormatting.FormatTemperatureFahrenheit(celsius));
    }

    [Theory]
    [InlineData(15, "15 min")]
    [InlineData(60, "1 hr")]
    [InlineData(120, "2 hr")]
    [InlineData(75, "1 hr 15 min")]
    [InlineData(90, "1 hr 30 min")]
    public void FormatDurationCompact_formats_correctly(int minutes, string expected)
    {
        Assert.Equal(expected, RoastDisplayFormatting.FormatDurationCompact(minutes));
    }

    [Fact]
    public void FormatTemperatureFahrenheitDisplay_includes_unit()
    {
        Assert.Equal("355 °F", RoastDisplayFormatting.FormatTemperatureFahrenheitDisplay(180));
    }
}
