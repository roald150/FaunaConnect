using FaunaConnect2.App.Models;
using FaunaConnect2.App.Services;

namespace FaunaConnect2.App.Tests;

public class SpatialServiceTests
{
    private readonly SpatialService _service = new();

    [Fact]
    public void IsPointInPolygon_PointInsideSquare_ReturnsTrue()
    {
        var square = new List<CoordinateDto>
        {
            new() { Latitude = 0, Longitude = 0 },
            new() { Latitude = 0, Longitude = 10 },
            new() { Latitude = 10, Longitude = 10 },
            new() { Latitude = 10, Longitude = 0 }
        };
        var result = _service.IsPointInPolygon(5, 5, square);
        Assert.True(result);
    }

    [Fact]
    public void IsPointInPolygon_PointOutsideSquare_ReturnsFalse()
    {
        var square = new List<CoordinateDto>
        {
            new() { Latitude = 0, Longitude = 0 },
            new() { Latitude = 0, Longitude = 10 },
            new() { Latitude = 10, Longitude = 10 },
            new() { Latitude = 10, Longitude = 0 }
        };
        var result = _service.IsPointInPolygon(15, 15, square);
        Assert.False(result);
    }

    [Fact]
    public void IsPointInPolygon_NullPolygon_ReturnsFalse()
    {
        var result = _service.IsPointInPolygon(5, 5, null!);
        Assert.False(result);
    }

    [Fact]
    public void IsPointInPolygon_LessThan3Points_ReturnsFalse()
    {
        var line = new List<CoordinateDto>
        {
            new() { Latitude = 0, Longitude = 0 },
            new() { Latitude = 10, Longitude = 10 }
        };
        var result = _service.IsPointInPolygon(5, 5, line);
        Assert.False(result);
    }

    [Fact]
    public void IsWithinRadius_PointWithinRadius_ReturnsTrue()
    {
        var result = _service.IsWithinRadius(51.0, 5.0, 51.001, 5.001, 200);
        Assert.True(result);
    }

    [Fact]
    public void IsWithinRadius_PointOutsideRadius_ReturnsFalse()
    {
        var result = _service.IsWithinRadius(51.0, 5.0, 52.0, 6.0, 1000);
        Assert.False(result);
    }
}
