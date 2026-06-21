using FaunaConnect2.Api.Helpers;
using Xunit;

namespace FaunaConnect2.Api.Tests;

public class WindDirectionTest
{
    [Theory]
    [InlineData(0, "N")]
    [InlineData(45, "NE")]
    [InlineData(90, "E")]
    [InlineData(135, "SE")]
    [InlineData(180, "S")]
    [InlineData(225, "SW")]
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
