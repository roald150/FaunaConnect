using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Globalization;
namespace FaunaConnect2.Api.Controllers;

// Dit zijn de simpele objecten die we naar MAUI sturen
    public class ParcelDto
    {
        public string ParcelNumber { get; set; } = string.Empty;
        public double AreaInSquareMeters { get; set; }
        // De lijst met punten om het jachtveld mee te tekenen op de kaart
        public List<CoordinateDto> Coordinates { get; set; } = new();
    }

    public class CoordinateDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ParcelsController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public ParcelsController()
        {
            _httpClient = new HttpClient();
            // PDOK vereist een 'User-Agent' header zodat ze weten welke app de data opvraagt
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "FaunaConnect2-AssessmentApp");
        }

        // GET: api/parcels?lat=51.650&lng=5.050
        [HttpGet]
        public async Task<IActionResult> GetParcelsByLocation([FromQuery] double lat, [FromQuery] double lng)
        {
            // 1. Maak een Bounding Box (zoekregio) rondom de kliklocatie
            // We vergroten de delta naar 0.003 (~300m) om meer omringende percelen te tonen
            double delta = 0.003; 
            double minLng = lng - delta;
            double minLat = lat - delta;
            double maxLng = lng + delta;
            double maxLat = lat + delta;

            // Zorg dat de getallen met een punt (.) worden geschreven en niet met een komma (,)
            string bbox = $"{minLng.ToString(CultureInfo.InvariantCulture)}," +
                          $"{minLat.ToString(CultureInfo.InvariantCulture)}," +
                          $"{maxLng.ToString(CultureInfo.InvariantCulture)}," +
                          $"{maxLat.ToString(CultureInfo.InvariantCulture)}";

            // 2. De officiële live URL van de PDOK OGC API
            string pdokUrl = $"https://api.pdok.nl/kadaster/brk-kadastrale-kaart/ogc/v1/collections/perceel/items?bbox={bbox}";

            try
            {
                // 3. Schiet het verzoek af naar de overheid (PDOK)
                var response = await _httpClient.GetAsync(pdokUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Fout bij ophalen van PDOK data.");
                }

                string geoJsonString = await response.Content.ReadAsStringAsync();

                // 4. Haal alleen de belangrijke info uit de enorme JSON brij
                var simplifiedParcels = ParseGeoJson(geoJsonString);

                return Ok(simplifiedParcels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fout in de API: {ex.Message}");
            }
        }

        // GET: api/parcels/bbox?minLat=51.64&minLng=5.04&maxLat=51.66&maxLng=5.06
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
                var response = await _httpClient.GetAsync(pdokUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Fout bij ophalen van PDOK data.");
                }

                string geoJsonString = await response.Content.ReadAsStringAsync();
                var simplifiedParcels = ParseGeoJson(geoJsonString);

                return Ok(simplifiedParcels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fout in de API: {ex.Message}");
            }
        }

        // Hulpmethode om door de ingewikkelde GeoJSON structuur te wandelen
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
                            parcelDto.ParcelNumber = idElement.GetString() ?? "Onbekend";
                        else if (props.TryGetProperty("lokaalID", out JsonElement lokaalIdElement))
                            parcelDto.ParcelNumber = lokaalIdElement.GetString() ?? "Onbekend";

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
                                // Bij MultiPolygon pakken we voor nu even het eerste polygoon-deel
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

            // De eerste ring is de buitenste grens (outer ring)
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