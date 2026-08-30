using MeatyTimes.Core;
using MeatyTimes.Core.Domain;
using MeatyTimes.Web.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MeatyTimes.Web.Tests;

public class RoastServiceTests
{
    private static RoastService CreateService()
    {
        var services = new ServiceCollection();
        services.AddMeatyTimesCore();
        services.AddSingleton<RoastService>();
        return services.BuildServiceProvider().GetRequiredService<RoastService>();
    }

    [Fact]
    public void GetMeats_returns_all_supported_meat_types()
    {
        var service = CreateService();

        var meats = service.GetMeats();

        Assert.Equal(5, meats.Count);
        Assert.Contains(meats, m => m.Id == "beef" && m.SupportsDoneness);
        Assert.Contains(meats, m => m.Id == "chicken" && !m.SupportsDoneness);
    }

    [Fact]
    public void Calculate_returns_cooking_result_for_valid_beef()
    {
        var service = CreateService();

        var result = service.Calculate("beef", 2.0m, "Medium");

        Assert.Equal("beef", result.MeatType);
        Assert.True(result.TotalCookingMinutes > 0);
        Assert.NotEmpty(result.Phases);
    }

    [Fact]
    public void Calculate_maps_weight_validation_to_service_exception()
    {
        var service = CreateService();

        var ex = Assert.Throws<RoastServiceException>(() =>
            service.Calculate("beef", 0.1m, "Medium"));

        Assert.Equal("weightKg", ex.Errors.Keys.Single());
        Assert.Contains("Minimum weight", ex.Errors["weightKg"][0]);
        Assert.Equal(ex.Errors["weightKg"][0], ex.FirstError);
    }

    [Fact]
    public void Calculate_maps_doneness_validation_to_service_exception()
    {
        var service = CreateService();

        var ex = Assert.Throws<RoastServiceException>(() =>
            service.Calculate("beef", 2.0m, null));

        Assert.Equal("doneness", ex.Errors.Keys.Single());
        Assert.Contains("Select a doneness level", ex.Errors["doneness"][0]);
    }

    [Fact]
    public void Calculate_maps_invalid_meat_type_to_service_exception()
    {
        var service = CreateService();

        var ex = Assert.Throws<RoastServiceException>(() =>
            service.Calculate("turkey", 2.0m, "Medium"));

        Assert.Equal("meatType", ex.Errors.Keys.Single());
    }

    [Fact]
    public void PlanSchedule_returns_achievable_schedule()
    {
        var service = CreateService();
        var servingTime = DateTimeOffset.UtcNow.AddHours(3);

        var schedule = service.PlanSchedule("beef", 2.0m, "Medium", servingTime);

        Assert.True(schedule.IsAchievable);
        Assert.NotNull(schedule.StartCookingTime);
        Assert.Equal(servingTime, schedule.ServingTime);
    }

    [Fact]
    public void PlanSchedule_maps_past_serving_time_to_service_exception()
    {
        var service = CreateService();
        var pastTime = DateTimeOffset.UtcNow.AddMinutes(-5);

        var ex = Assert.Throws<RoastServiceException>(() =>
            service.PlanSchedule("beef", 2.0m, "Medium", pastTime));

        Assert.Equal("servingTime", ex.Errors.Keys.Single());
        Assert.Contains("future", ex.Errors["servingTime"][0], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void FromValidation_without_field_uses_message_only()
    {
        var exception = RoastServiceException.FromValidation(
            new RoastValidationException("Something went wrong."));

        Assert.Empty(exception.Errors);
        Assert.Equal("Something went wrong.", exception.FirstError);
    }
}
