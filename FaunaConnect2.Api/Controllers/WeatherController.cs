using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace FaunaConnect2.Api.Controllers;

[ApiController]
[Route("api")]
public class WeatherController : ControllerBase
{
    private readonly HttpClient _httpClient;
    // In een echte app zou deze key in appsettings.json staan
    private const string ApiKey = "GEEN_KEY_NODIG_VOOR_DEMO_OF_MOCK"; 

    public WeatherController()
    {
        _httpClient = new HttpClient();
    }

    [HttpGet("weather")]
    public async Task<IActionResult> GetWeather(double lat, double lng)
    {
        // Voor het assessment geven we even simpele mock-data terug 
        // zodat de app altijd werkt zonder API key gedoe.
        var mockWeather = new 
        {
            Temp = 18.5,
            WindSpeed = 3.2,
            WindDirection = 240, // graden (ZW)
            Condition = "Onbewolkt"
        };
        return Ok(mockWeather);
    }

    [HttpGet("sun")]
    public async Task<IActionResult> GetSun(double lat, double lng)
    {
        // Proxy naar de gratis Sunrise-Sunset API
        string url = $"https://api.sunrise-sunset.org/json?lat={lat}&lng={lng}&formatted=0";
        var response = await _httpClient.GetFromJsonAsync<SunResponse>(url);
        
        if (response?.Results == null) return BadRequest();

        return Ok(new 
        {
            Sunrise = response.Results.Sunrise.ToLocalTime(),
            Sunset = response.Results.Sunset.ToLocalTime()
        });
    }

    private class SunResponse
    {
        public SunResults? Results { get; set; }
    }

    private class SunResults
    {
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
    }
}