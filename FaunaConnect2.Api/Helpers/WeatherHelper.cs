namespace FaunaConnect2.Api.Helpers;

public static class WeatherHelper
{
    public static string GetWindDirection(double degrees)
    {
        string[] cardinals = { "N", "NE", "E", "SE", "S", "SW", "W", "NW", "N" };
        return cardinals[(int)Math.Round(((double)degrees % 360) / 45)];
    }
}