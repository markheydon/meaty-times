using Bunit;
using MeatyTimes.Web.Components.Roast;
using MeatyTimes.Web.Services;
using MudBlazor.Services;

namespace MeatyTimes.Web.Tests;

public class RoastResultsDisplayTests : BunitContext
{
    private static readonly IReadOnlyList<RoastApiClient.MeatTypeDto> Meats =
    [
        new("beef", "Beef", true, ["Rare", "Medium", "WellDone"], 0.5m, 15m),
        new("chicken", "Chicken", false, [], 0.8m, 10m),
    ];

    public RoastResultsDisplayTests()
    {
        Services.AddMudServices();
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
        Assert.Contains("Calculation for", markup);
        Assert.Contains("Meat type:", markup);
        Assert.Contains("Beef", markup);
        Assert.Contains("Weight (kg):", markup);
        Assert.Contains("2.0 kg", markup);
        Assert.Contains("Doneness:", markup);
        Assert.Contains("Medium", markup);
        Assert.Contains("Roasting instructions", markup);
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
    public void Renders_nothing_when_result_is_null()
    {
        var input = new RoastInputForm.RoastInputModel("beef", 2.0m, "Medium");

        var cut = Render<RoastResultsDisplay>(parameters => parameters
            .Add(p => p.Result, (RoastApiClient.CookingResultDto?)null)
            .Add(p => p.Input, input)
            .Add(p => p.Meats, Meats));

        Assert.DoesNotContain("Calculation for", cut.Markup);
        Assert.DoesNotContain("Roasting instructions", cut.Markup);
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
        // Page-level preservation is handled in RoastCalculator.HandleCalculate.
        var input = new RoastInputForm.RoastInputModel("beef", 2.0m, "Medium");
        var result = CreateResult();

        var cut = Render<RoastResultsDisplay>(parameters => parameters
            .Add(p => p.Result, result)
            .Add(p => p.Input, input)
            .Add(p => p.Meats, Meats));

        Assert.Contains("Beef", cut.Markup);
        Assert.Contains("2.0 kg", cut.Markup);
        Assert.Contains("Roasting instructions", cut.Markup);
    }

    private static RoastApiClient.CookingResultDto CreateResult(string meatType = "beef") =>
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
}
