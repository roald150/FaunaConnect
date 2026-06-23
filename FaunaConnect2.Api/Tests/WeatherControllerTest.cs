using Microsoft.AspNetCore.Mvc;
using FaunaConnect2.Api.Controllers;
using Xunit;

namespace FaunaConnect2.Api.Tests;

public class WeatherControllerTest
{
    [Fact]
    public async Task GetWeather_ShouldReturnMockData()
    {
        var controller = new WeatherController();

        var result = await controller.GetWeather(51.65, 5.05);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }
}
