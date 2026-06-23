using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace FaunaConnect2.Api.Controllers;

// These are simple objects we send to MAUI
public class ParcelDto
{
    public string ParcelNumber { get; set; } = string.Empty;
    public double AreaInSquareMeters { get; set; }
    public List<CoordinateDto> Coordinates { get; set; } = [];
}

public class CoordinateDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ParcelsController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetParcelsByLocation([FromQuery] double lat, [FromQuery] double lng)
    {
        double delta = 0.003; 
        double minLng = lng - delta;
        double minLat = lat - delta;
        double maxLng = lng + delta;
        double maxLat = lat + delta;

        string bbox = $"{minLng.ToString(CultureInfo.InvariantCulture)}," +
                      $"{minLat.ToString(CultureInfo.InvariantCulture)}," +
                      $"{maxLng.ToString(CultureInfo.InvariantCulture)}," +
                      $"{maxLat.ToString(CultureInfo.InvariantCulture)}";

        string pdokUrl = $"https://api.pdok.nl/kadaster/brk-kadastrale-kaart/ogc/v1/collections/perceel/items?bbox={bbox}";

        try
        {
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("User-Agent", "FaunaConnect2");

            var response = await client.GetAsync(pdokUrl);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Error retrieving PDOK data.");
            }

            string geoJsonString = await response.Content.ReadAsStringAsync();

            var simplifiedParcels = ParseGeoJson(geoJsonString);

            return Ok(simplifiedParcels);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"API Error: {ex.Message}");
        }
    }

    [HttpGet("bbox")]
    public async Task<IActionResult> GetParcelsByBounds([FromQuery] double minLat, [FromQuery] double minLng, [FromQuery] double maxLat, [FromQuery] double maxLng)
    {
        string bbox = $"{minLng.ToString(CultureInfo.InvariantCulture)}," +
                      $"{minLat.ToString(CultureInfo.InvariantCulture)}," +
                      $"{maxLng.ToString(CultureInfo.InvariantCulture)}," +
                      $"{maxLat.ToString(CultureInfo.InvariantCulture)}";

        string pdokUrl = $"https://api.pdok.nl/kadaster/brk-kadastrale-kaart/ogc/v1/collections/perceel/items?bbox={bbox}&limit=100";

        try
        {
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("User-Agent", "FaunaConnect2-AssessmentApp");

            var response = await client.GetAsync(pdokUrl);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Error retrieving PDOK data.");
            }

            string geoJsonString = await response.Content.ReadAsStringAsync();
            var simplifiedParcels = ParseGeoJson(geoJsonString);

            return Ok(simplifiedParcels);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"API Error: {ex.Message}");
        }
    }

    private List<ParcelDto> ParseGeoJson(string jsonString)
    {
        var parcels = new List<ParcelDto>();

        using JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement root = doc.RootElement;

        if (root.TryGetProperty("features", out JsonElement features) && features.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement feature in features.EnumerateArray())
            {
                var parcelDto = new ParcelDto();

                if (feature.TryGetProperty("properties", out JsonElement props))
                {
                    if (props.TryGetProperty("identificatie_lokaal_id", out JsonElement idElement))
                        parcelDto.ParcelNumber = idElement.GetString() ?? "Unknown";
                    else if (props.TryGetProperty("lokaalID", out JsonElement lokaalIdElement))
                        parcelDto.ParcelNumber = lokaalIdElement.GetString() ?? "Unknown";

                    if (props.TryGetProperty("kadastrale_grootte_waarde", out JsonElement sizeElement))
                        parcelDto.AreaInSquareMeters = sizeElement.GetDouble();
                    else if (props.TryGetProperty("kadastraleGrootte", out JsonElement sizeElementLegacy))
                        parcelDto.AreaInSquareMeters = sizeElementLegacy.GetDouble();
                }

                if (feature.TryGetProperty("geometry", out JsonElement geom))
                {
                    string? type = geom.GetProperty("type").GetString();
                    if (geom.TryGetProperty("coordinates", out JsonElement coordinates) && coordinates.ValueKind == JsonValueKind.Array)
                    {
                        if (type == "Polygon")
                        {
                            ParsePolygon(coordinates, parcelDto);
                        }
                        else if (type == "MultiPolygon")
                        {
                            if (coordinates.GetArrayLength() > 0)
                            {
                                ParsePolygon(coordinates[0], parcelDto);
                            }
                        }
                    }
                }

                parcels.Add(parcelDto);
            }
        }

        return parcels;
    }

    private void ParsePolygon(JsonElement polygonCoords, ParcelDto dto)
    {
        if (polygonCoords.ValueKind != JsonValueKind.Array || polygonCoords.GetArrayLength() == 0) return;

        JsonElement outerRing = polygonCoords[0];
        if (outerRing.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement point in outerRing.EnumerateArray())
            {
                if (point.ValueKind == JsonValueKind.Array && point.GetArrayLength() >= 2)
                {
                    dto.Coordinates.Add(new CoordinateDto
                    {
                        Longitude = point[0].GetDouble(),
                        Latitude = point[1].GetDouble()
                    });
                }
            }
        }
    }
}
