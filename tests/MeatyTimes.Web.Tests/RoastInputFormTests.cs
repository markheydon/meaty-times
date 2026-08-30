using Bunit;
using MeatyTimes.Web.Components.Roast;
using MeatyTimes.Web.Services;

namespace MeatyTimes.Web.Tests;

public class RoastInputFormTests : BunitContext
{
    private static readonly IReadOnlyList<MeatTypeDto> Meats =
    [
        new("beef", "Beef", true, ["Rare", "Medium", "WellDone"], 0.5m, 15m),
        new("chicken", "Chicken", false, [], 0.8m, 10m),
    ];

    public RoastInputFormTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Renders_your_roast_title_and_calculate_times_button()
    {
        var cut = Render<RoastInputForm>(parameters => parameters
            .Add(p => p.Meats, Meats));

        Assert.Contains("Your Roast", cut.Markup);
        Assert.Contains("Calculate Times", cut.Markup);
        Assert.Contains("<select", cut.Markup);
        Assert.Contains("type=\"number\"", cut.Markup);
        Assert.Contains("kg", cut.Markup);
    }

    [Fact]
    public void Hides_doneness_when_meat_does_not_support_it()
    {
        var meats = new List<MeatTypeDto> { Meats[1] };
        var cut = Render<RoastInputForm>(parameters => parameters
            .Add(p => p.Meats, meats));

        var selectElements = cut.FindAll("select");
        Assert.Single(selectElements);
    }

    [Fact]
    public void Shows_doneness_select_for_beef()
    {
        var cut = Render<RoastInputForm>(parameters => parameters
            .Add(p => p.Meats, Meats));

        cut.Find("#meat-type").Change("beef");
        cut.Render();

        var selectElements = cut.FindAll("select");
        Assert.Equal(2, selectElements.Count);
        Assert.Contains("Doneness", cut.Markup);
    }
}
