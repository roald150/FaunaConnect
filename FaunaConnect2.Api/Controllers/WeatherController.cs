using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;

namespace FaunaConnect2.Api.Controllers;

[Authorize]
[ApiController]
[Route("api")]
public class WeatherController : ControllerBase
{
    private readonly HttpClient _httpClient;
    
    private const string ApiKey = ""; 

    public WeatherController()
    {
        _httpClient = new HttpClient();
    }

    [HttpGet("weather")]
    public async Task<IActionResult> GetWeather(double lat, double lng)
    {
        // testdata for weather, since the OpenWeatherMap API requires an API key and is not free
        var mockWeather = new 
        {
            Temp = 18.5,
            WindSpeed = 3.2,
            WindDirection = 240, // degrees (SW)
            Condition = "Clear"
        };
        return Ok(mockWeather);
    }

    [HttpGet("sun")]
    public async Task<IActionResult> GetSun(double lat, double lng)
    {
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