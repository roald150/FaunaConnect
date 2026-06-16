namespace FaunaConnect2.Api.Helpers;

public static class WeatherHelper
{
    public static string GetWindDirection(double degrees)
    {
        string[] cardinals = { "N", "NO", "O", "ZO", "Z", "ZW", "W", "NW", "N" };
        return cardinals[(int)Math.Round(((double)degrees % 360) / 45)];
    }
}