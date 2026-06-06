using System.Net.Http.Json;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace FaunaConnect2.App;

// Hulpklassen om de JSON data van onze eigen API op te vangen
public class ParcelResponseDto
{
    public string ParcelNumber { get; set; } = string.Empty;
    public double AreaInSquareMeters { get; set; }
    public List<CoordinateDto> Coordinates { get; set; } = new();
}

public class CoordinateDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public partial class MapPage : ContentPage
{
    private readonly HttpClient _httpClient;

    public MapPage()
    {
        InitializeComponent();
        
        // We maken verbinding met jouw API poort 5282!
        string baseUri = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5282/api/" : "http://localhost:5282/api/";
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUri) };

        // Start de kaart op een mooie centrale locatie in Nederland (bijv. Noord-Brabant)
        var centralLocation = new Location(51.650, 5.050);
        var mapSpan = MapSpan.FromCenterAndRadius(centralLocation, Distance.FromKilometers(2));
        JachtMap.MoveToRegion(mapSpan);
    }

    private async void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        // 1. Wis oude jachtvelden en cirkels van de kaart
        JachtMap.MapElements.Clear();

        // Haal de geklikte coördinaten op
        double lat = e.Location.Latitude;
        double lng = e.Location.Longitude;

        try
        {
            // 2. Roep onze eigen API aan, die live de OGC API van het Kadaster bevrraagt!
            var response = await _httpClient.GetFromJsonAsync<List<ParcelResponseDto>>($"parcels?lat={lat}&lng={lng}");

            if (response != null && response.Count > 0)
            {
                var targetParcel = response[0]; // Pak het dichtstbijzijnde perceel

                // Update de UI teksten
                ParcelIdLabel.Text = $"ID: {targetParcel.ParcelNumber}";
                ParcelSizeLabel.Text = $"Grootte: {targetParcel.AreaInSquareMeters:N0} m² ({(targetParcel.AreaInSquareMeters / 10000):F2} ha)";

                // 3. USER STORY 01 & 02: Teken de Kadastrale grens (Polygoon)
                var polygon = new Polygon
                {
                    StrokeColor = Colors.Blue,
                    StrokeWidth = 3,
                    FillColor = Color.FromRgba(0, 0, 255, 30) // Transparant blauw jachtveld
                };

                foreach (var coord in targetParcel.Coordinates)
                {
                    polygon.Geopath.Add(new Location(coord.Latitude, coord.Longitude));
                }
                JachtMap.MapElements.Add(polygon);

                // 4. USER STORY 03: Teken de wettelijke 300 meter schietcirkel rondom de jager
                var shootingCircle = new Circle
                {
                    Center = new Location(lat, lng),
                    Radius = Distance.FromMeters(300),
                    StrokeColor = Colors.Red,
                    StrokeWidth = 2,
                    FillColor = Color.FromRgba(255, 0, 0, 20) // Transparant rood waarschuwingsgebied
                };
                JachtMap.MapElements.Add(shootingCircle);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Perceelgegevens konden niet worden opgehaald: {ex.Message}", "OK");
        }
    }
}