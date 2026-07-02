using MeatyTimes.Core.Calculation;
using MeatyTimes.Core.Rules;

namespace MeatyTimes.Core.Tests;

public static class TestFixtures
{
    public static RoastCalculator CreateCalculator() =>
        new(new CookingRuleLoader());

    public static ScheduleCalculator CreateScheduleCalculator() =>
        new(CreateCalculator());
}
