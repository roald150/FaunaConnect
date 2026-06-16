namespace FaunaConnect2.Api.Helpers;

public static class WeatherHelper
{
    /// <summary>
    /// Converts meteorological degrees to cardinal wind direction.
    /// </summary>
    /// <param name="degrees">Wind direction in degrees.</param>
    /// <returns>Cardinal direction string (e.g., "N", "NE", "E").</returns>
    public static string GetWindDirection(double degrees)
    {
        // Cardinal points: North, North-East, East, South-East, South, South-West, West, North-West
        string[] cardinals = { "N", "NE", "E", "SE", "S", "SW", "W", "NW", "N" };
        return cardinals[(int)Math.Round(((double)degrees % 360) / 45)];
    }
}