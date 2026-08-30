using MeatyTimes.Core.Domain;

namespace MeatyTimes.Core.Tests;

public class RoastRequestFromInputsTests
{
    [Fact]
    public void Valid_beef_inputs_return_request()
    {
        var request = RoastRequest.FromInputs("beef", 2.0m, "Medium");

        Assert.Equal(MeatTypeId.Beef, request.MeatType);
        Assert.Equal(2.0m, request.WeightKg);
        Assert.Equal(Doneness.Medium, request.Doneness);
    }

    [Fact]
    public void Chicken_without_doneness_returns_null_doneness()
    {
        var request = RoastRequest.FromInputs("chicken", 1.5m, null);

        Assert.Equal(MeatTypeId.Chicken, request.MeatType);
        Assert.Null(request.Doneness);
    }

    [Fact]
    public void Invalid_meat_type_throws_validation_error()
    {
        var ex = Assert.Throws<RoastValidationException>(() =>
            RoastRequest.FromInputs("turkey", 2.0m, "Medium"));

        Assert.Equal("meatType", ex.Field);
        Assert.Contains("Unsupported meat type", ex.Message);
    }

    [Fact]
    public void Invalid_doneness_throws_validation_error()
    {
        var ex = Assert.Throws<RoastValidationException>(() =>
            RoastRequest.FromInputs("beef", 2.0m, "Blue"));

        Assert.Equal("doneness", ex.Field);
        Assert.Contains("Unsupported doneness level", ex.Message);
    }
}
