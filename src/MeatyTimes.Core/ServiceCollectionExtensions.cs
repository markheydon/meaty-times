using MeatyTimes.Core.Calculation;
using MeatyTimes.Core.Rules;
using Microsoft.Extensions.DependencyInjection;

namespace MeatyTimes.Core;

/// <summary>
/// Registers MeatyTimes domain services.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMeatyTimesCore(this IServiceCollection services)
    {
        services.AddSingleton<CookingRuleLoader>();
        services.AddSingleton<RoastCalculator>();
        services.AddSingleton<ScheduleCalculator>();
        return services;
    }
}
