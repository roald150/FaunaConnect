using FaunaConnect2.Api.Helpers;
using Xunit;

namespace FaunaConnect2.Api.Tests;

public class WindDirectionTest
{
    [Theory]
    [InlineData(0, "N")]
    [InlineData(45, "NO")]
    [InlineData(90, "O")]
    [InlineData(135, "ZO")]
    [InlineData(180, "Z")]
    [InlineData(225, "ZW")]
    [InlineData(270, "W")]
    [InlineData(315, "NW")]
    [InlineData(360, "N")]
    public void GetWindDirection_ShouldReturnCorrectCardinal(double degrees, string expected)
    {
        // Act
        var result = WeatherHelper.GetWindDirection(degrees);

        // Assert
        Assert.Equal(expected, result);
    }
}
