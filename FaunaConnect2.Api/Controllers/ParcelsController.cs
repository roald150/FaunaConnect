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
            // 1. Maak een kleine Bounding Box (zoekregio) rondom de kliklocatie
            // We trekken een klein beetje van de lat/lng af en tellen er wat bij op
            double delta = 0.001; 
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

        // Hulpmethode om door de ingewikkelde GeoJSON structuur te wandelen
        private List<ParcelDto> ParseGeoJson(string jsonString)
        {
            var parcels = new List<ParcelDto>();

            // JsonDocument is ideaal om flexibel door diepe JSON te bladeren zonder grote bibliotheken
            using JsonDocument doc = JsonDocument.Parse(jsonString);
            JsonElement root = doc.RootElement;

            if (root.TryGetProperty("features", out JsonElement features) && features.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement feature in features.EnumerateArray())
                {
                    var parcelDto = new ParcelDto();

                    // A. Haal de gegevens van het perceel op
                    if (feature.TryGetProperty("properties", out JsonElement props))
                    {
                        if (props.TryGetProperty("lokaalID", out JsonElement idElement))
                            parcelDto.ParcelNumber = idElement.GetString() ?? "Onbekend";

                        if (props.TryGetProperty("kadastraleGrootte", out JsonElement sizeElement))
                            parcelDto.AreaInSquareMeters = sizeElement.GetDouble();
                    }

                    // B. Haal de geografische vormen (polygonen) op om te kunnen intekenen
                    if (feature.TryGetProperty("geometry", out JsonElement geom))
                    {
                        if (geom.TryGetProperty("coordinates", out JsonElement coordinates) && coordinates.ValueKind == JsonValueKind.Array)
                        {
                            // GeoJSON polygons hebben een extra geneste array voor de buitenste ring
                            foreach (JsonElement ring in coordinates.EnumerateArray())
                            {
                                foreach (JsonElement point in ring.EnumerateArray())
                                {
                                    if (point.GetArrayLength() >= 2)
                                    {
                                        // GeoJSON structuur is altijd eerst [Longitude, Latitude]
                                        double longitude = point[0].GetDouble();
                                        double latitude = point[1].GetDouble();

                                        parcelDto.Coordinates.Add(new CoordinateDto { Latitude = latitude, Longitude = longitude });
                                    }
                                }
                                break; // We pakken alleen de buitenste grens, eventuele 'gaten' in het perceel negeren we simpelheidshalve
                            }
                        }
                    }

                    parcels.Add(parcelDto);
                }
            }

            return parcels;
        }
    }