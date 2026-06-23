namespace FaunaConnect2.App.Services;

using FaunaConnect2.App.Models;
using Microsoft.Maui.Devices.Sensors;

public class SpatialService
{
    /// Checks if a point is inside a polygon using the Ray Casting algorithm.
    public bool IsPointInPolygon(double lat, double lng, List<CoordinateDto> polygon)
    {
        if (polygon == null || polygon.Count < 3) return false;

        bool isInside = false;
        for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
        {
            if (((polygon[i].Latitude > lat) != (polygon[j].Latitude > lat)) &&
                (lng < (polygon[j].Longitude - polygon[i].Longitude) * (lat - polygon[i].Latitude) / (polygon[j].Latitude - polygon[i].Latitude) + polygon[i].Longitude))
            {
                isInside = !isInside;
            }
        }
        return isInside;
    }

    /// Calculates the distance between two points in meters using the Haversine formula.
    public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        return Location.CalculateDistance(lat1, lon1, lat2, lon2, DistanceUnits.Kilometers) * 1000;
    }

    /// Checks if a point is within a certain radius of another point.
    public bool IsWithinRadius(double lat1, double lon1, double lat2, double lon2, double radiusMeters)
    {
        return CalculateDistance(lat1, lon1, lat2, lon2) <= radiusMeters;
    }
}
