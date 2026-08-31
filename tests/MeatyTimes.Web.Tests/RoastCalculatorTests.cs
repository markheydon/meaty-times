using Bunit;
using MeatyTimes.Core;
using MeatyTimes.Web.Components.Pages;
using MeatyTimes.Web.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MeatyTimes.Web.Tests;

public class RoastCalculatorTests : BunitContext
{
    public RoastCalculatorTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddMeatyTimesCore();
        Services.AddSingleton<RoastService>();
    }

    [Fact]
    public void PlanSchedule_failure_clears_stale_schedule_milestones()
    {
        var cut = Render<RoastCalculator>();
        cut.WaitForAssertion(() => Assert.Contains("Calculate Times", cut.Markup));

        var futureLocal = DateTime.Now.AddHours(4).ToString("yyyy-MM-ddTHH:mm");
        cut.Find("#serve-at").Change(futureLocal);
        cut.Find("button").Click();

        cut.WaitForAssertion(() => Assert.Contains("Start cooking", cut.Markup));

        var pastLocal = DateTime.Now.AddHours(-1).ToString("yyyy-MM-ddTHH:mm");
        cut.Find("#serve-at").Change(pastLocal);
        cut.Find("button").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("future", cut.Markup, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Start cooking", cut.Markup);
        });
    }

    [Fact]
    public void Weight_validation_shows_single_field_error_without_general_banner()
    {
        var cut = Render<RoastCalculator>();
        cut.WaitForAssertion(() => Assert.Contains("Calculate Times", cut.Markup));

        cut.Find("#weight-kg").Change("0.1");
        cut.Find("button").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("Minimum weight", cut.Markup);
            Assert.Equal(1, CountOccurrences(cut.Markup, "Minimum weight"));
            Assert.DoesNotContain("mb-4 rounded-lg bg-red-50 border border-red-200 p-4 text-sm text-red-800", cut.Markup);
        });
    }

    [Fact]
    public void Calculate_without_serve_at_hides_schedule_after_prior_successful_plan()
    {
        var cut = Render<RoastCalculator>();
        cut.WaitForAssertion(() => Assert.Contains("Calculate Times", cut.Markup));

        var futureLocal = DateTime.Now.AddHours(4).ToString("yyyy-MM-ddTHH:mm");
        cut.Find("#serve-at").Change(futureLocal);
        cut.Find("button").Click();
        cut.WaitForAssertion(() => Assert.Contains("Start cooking", cut.Markup));

        cut.Find("#serve-at").Change(string.Empty);
        cut.Find("button").Click();

        cut.WaitForAssertion(() => Assert.DoesNotContain("Start cooking", cut.Markup));
    }

    private static int CountOccurrences(string source, string value)
    {
        var count = 0;
        var index = 0;

        while ((index = source.IndexOf(value, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += value.Length;
        }

        return count;
    }
}
