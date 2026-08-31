using Bunit;
using MeatyTimes.Web.Components.Roast;
using MeatyTimes.Web.Services;

namespace MeatyTimes.Web.Tests;

public class RoastResultsDisplayTests : BunitContext
{
    private static readonly IReadOnlyList<MeatTypeDto> Meats =
    [
        new("beef", "Beef", true, ["Rare", "Medium", "WellDone"], 0.5m, 15m),
        new("chicken", "Chicken", false, [], 0.8m, 10m),
    ];

    public RoastResultsDisplayTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Renders_input_summary_with_display_name_weight_and_doneness()
    {
        var input = new RoastInputForm.RoastInputModel("beef", 2.0m, "Medium");
        var result = CreateResult();

        var cut = Render<RoastResultsDisplay>(parameters => parameters
            .Add(p => p.Result, result)
            .Add(p => p.Input, input)
            .Add(p => p.Meats, Meats));

        var markup = cut.Markup;
        Assert.Contains("Roasting Instructions", markup);
        Assert.Contains("Meat type:", markup);
        Assert.Contains("Beef", markup);
        Assert.Contains("Weight:", markup);
        Assert.Contains("2.0 kg", markup);
        Assert.Contains("Doneness:", markup);
        Assert.Contains("Medium", markup);
        Assert.Contains("estimated", markup);
    }

    [Fact]
    public void Hides_doneness_row_when_meat_does_not_support_doneness()
    {
        var input = new RoastInputForm.RoastInputModel("chicken", 1.8m, null);
        var result = CreateResult(meatType: "chicken");

        var cut = Render<RoastResultsDisplay>(parameters => parameters
            .Add(p => p.Result, result)
            .Add(p => p.Input, input)
            .Add(p => p.Meats, Meats));

        var markup = cut.Markup;
        Assert.Contains("Chicken", markup);
        Assert.Contains("1.8 kg", markup);
        Assert.DoesNotContain("Doneness:", markup);
    }

    [Fact]
    public void Shows_empty_chrome_when_result_is_null()
    {
        var cut = Render<RoastResultsDisplay>(parameters => parameters
            .Add(p => p.Result, (CookingResultDto?)null)
            .Add(p => p.Input, (RoastInputForm.RoastInputModel?)null)
            .Add(p => p.Meats, Meats));

        Assert.Contains("Roasting Instructions", cut.Markup);
        Assert.Contains("Enter your roast details", cut.Markup);
        Assert.DoesNotContain("Meat type:", cut.Markup);
        Assert.DoesNotContain("°C", cut.Markup);
    }

    [Fact]
    public void Summary_reflects_input_parameter_values_not_catalog_defaults()
    {
        var beefInput = new RoastInputForm.RoastInputModel("beef", 2.0m, "Medium");
        var lambInput = new RoastInputForm.RoastInputModel("lamb", 3.5m, "Rare");
        var meats = Meats.Concat([new("lamb", "Lamb", true, ["Rare", "Medium", "WellDone"], 0.5m, 12m)]).ToList();
        var result = CreateResult();

        var beefCut = Render<RoastResultsDisplay>(parameters => parameters
            .Add(p => p.Result, result)
            .Add(p => p.Input, beefInput)
            .Add(p => p.Meats, meats));

        var lambCut = Render<RoastResultsDisplay>(parameters => parameters
            .Add(p => p.Result, result)
            .Add(p => p.Input, lambInput)
            .Add(p => p.Meats, meats));

        Assert.Contains("Beef", beefCut.Markup);
        Assert.Contains("2.0 kg", beefCut.Markup);
        Assert.DoesNotContain("Lamb", beefCut.Markup);

        Assert.Contains("Lamb", lambCut.Markup);
        Assert.Contains("3.5 kg", lambCut.Markup);
        Assert.Contains("Rare", lambCut.Markup);
    }

    [Fact]
    public void Summary_remains_visible_when_result_and_input_remain_set()
    {
        var input = new RoastInputForm.RoastInputModel("beef", 2.0m, "Medium");
        var result = CreateResult();

        var cut = Render<RoastResultsDisplay>(parameters => parameters
            .Add(p => p.Result, result)
            .Add(p => p.Input, input)
            .Add(p => p.Meats, Meats));

        Assert.Contains("Beef", cut.Markup);
        Assert.Contains("2.0 kg", cut.Markup);
        Assert.Contains("Roasting Instructions", cut.Markup);
    }

    [Fact]
    public void Renders_each_cooking_phase_as_separate_row()
    {
        var input = new RoastInputForm.RoastInputModel("beef", 2.0m, "Medium");
        var result = CreateMultiPhaseResult();

        var cut = Render<RoastResultsDisplay>(parameters => parameters
            .Add(p => p.Result, result)
            .Add(p => p.Input, input)
            .Add(p => p.Meats, Meats));

        Assert.Contains("Roast at high heat", cut.Markup);
        Assert.Contains("Reduce temperature", cut.Markup);
        Assert.Contains("200 °C", cut.Markup);
        Assert.Contains("180 °C", cut.Markup);
    }

    [Fact]
    public void Temperature_rows_show_celsius_prominently_and_fahrenheit_secondary()
    {
        var input = new RoastInputForm.RoastInputModel("beef", 2.0m, "Medium");
        var result = CreateResult();

        var cut = Render<RoastResultsDisplay>(parameters => parameters
            .Add(p => p.Result, result)
            .Add(p => p.Input, input)
            .Add(p => p.Meats, Meats));

        Assert.Contains("180 °C", cut.Markup);
        Assert.Contains("355 °F", cut.Markup);
    }

    private static CookingResultDto CreateResult(string meatType = "beef") =>
        new(
            meatType,
            2.0m,
            "Medium",
            "TraditionalRoast",
            [new(1, "Roast", 180, 60)],
            60,
            20,
            80,
            "Test source");

    private static CookingResultDto CreateMultiPhaseResult() =>
        new(
            "beef",
            2.0m,
            "Medium",
            "TraditionalRoast",
            [
                new(1, "Roast at high heat", 200, 30),
                new(2, "Reduce temperature", 180, 45),
            ],
            75,
            20,
            95,
            "Test source");
}
