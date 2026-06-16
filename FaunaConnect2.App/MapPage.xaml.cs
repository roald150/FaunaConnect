using System.Net.Http.Json;
using System.Text.Json;
using FaunaConnect2.App.Models;
using FaunaConnect2.App.Services;

namespace FaunaConnect2.App;

public partial class MapPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private ParcelResponseDto? _selectedParcel;

    public MapPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient { BaseAddress = new Uri(UserService.BaseUrl) };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadHuntingGrounds();
        await LoadRegistrations();
        await LoadDamageReports();
    }

    private async Task LoadDamageReports()
    {
        if (DamageToggle == null || !DamageToggle.IsToggled)
        {
            await MapWebView.EvaluateJavaScriptAsync("drawDamageReports([])");
            return;
        }

        try
        {
            var reports = await _httpClient.GetFromJsonAsync<List<DamageReportDto>>("damagereports");
            if (reports != null)
            {
                string json = JsonSerializer.Serialize(reports);
                await MapWebView.EvaluateJavaScriptAsync($"drawDamageReports({json})");
            }
        }
        catch { }
    }

    private async void OnDamageToggleToggled(object? sender, ToggledEventArgs e)
    {
        await LoadDamageReports();
    }

    private async void OnReportDamageClicked(object? sender, EventArgs e)
    {
        // Navigeer naar de schademeldingspagina met de huidige locatie (indien geselecteerd)
        // Voor nu even simpel navigeren
        await Navigation.PushAsync(new DamageReportPage());
    }

    private async Task LoadHuntingGrounds()
    {
        try
        {
            // Ophalen van jachtgebieden voor de huidige jager
            string url = "huntinggrounds";
            if (UserService.CurrentUser != null && UserService.CurrentUser.Role == "Jager")
            {
                url += $"?userId={UserService.CurrentUser.Id}";
            }
            
            var grounds = await _httpClient.GetFromJsonAsync<List<HuntingGroundDto>>(url);
            if (grounds != null)
            {
                string json = JsonSerializer.Serialize(grounds);
                await MapWebView.EvaluateJavaScriptAsync($"drawHuntingGrounds({json})");

                // Bereken totale grootte
                double totalSize = grounds.Sum(g => g.TotalAreaInHectares);
                ParcelSizeLabel.Text = $"Totaal Jachtveld: {totalSize:F2} hectare";
            }
        }
        catch { }
    }

    private async Task LoadRegistrations()
    {
        try
        {
            var registrations = await _httpClient.GetFromJsonAsync<List<Registration>>("registrations");
            if (registrations != null)
            {
                string json = JsonSerializer.Serialize(registrations);
                await MapWebView.EvaluateJavaScriptAsync($"drawRegistrations({json})");
            }
        }
        catch { }
    }

    private async void OnWebViewNavigating(object? sender, WebNavigatingEventArgs e)
    {
        if (e.Url.StartsWith("invoke://mapclick"))
        {
            e.Cancel = true;
            
            var uri = new Uri(e.Url);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            if (double.TryParse(query["lat"], out double lat) && double.TryParse(query["lng"], out double lng))
            {
                await HandleMapClick(lat, lng);
            }
        }
        else if (e.Url.StartsWith("invoke://mapbounds"))
        {
            e.Cancel = true;
            var uri = new Uri(e.Url);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            if (double.TryParse(query["swLat"], out double swLat) && 
                double.TryParse(query["swLng"], out double swLng) &&
                double.TryParse(query["neLat"], out double neLat) &&
                double.TryParse(query["neLng"], out double neLng))
            {
                _ = LoadParcelsInBounds(swLat, swLng, neLat, neLng);
            }
        }
    }

    private async Task LoadParcelsInBounds(double swLat, double swLng, double neLat, double neLng)
    {
        try
        {
            await MainThread.InvokeOnMainThreadAsync(() => {
                MapLoadingIndicator.IsRunning = true;
                MapLoadingIndicator.IsVisible = true;
            });

            var url = $"parcels/bbox?minLat={swLat.ToString(System.Globalization.CultureInfo.InvariantCulture)}&minLng={swLng.ToString(System.Globalization.CultureInfo.InvariantCulture)}&maxLat={neLat.ToString(System.Globalization.CultureInfo.InvariantCulture)}&maxLng={neLng.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            var parcels = await _httpClient.GetFromJsonAsync<List<ParcelResponseDto>>(url);
            
            if (parcels != null && parcels.Count > 0)
            {
                string json = JsonSerializer.Serialize(parcels);
                await MainThread.InvokeOnMainThreadAsync(async () => {
                    await MapWebView.EvaluateJavaScriptAsync($"drawParcels({json}, null, true)");
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading parcels: {ex.Message}");
        }
        finally
        {
            await MainThread.InvokeOnMainThreadAsync(() => {
                MapLoadingIndicator.IsRunning = false;
                MapLoadingIndicator.IsVisible = false;
            });
        }
    }

    private async void OnCircleToggleToggled(object? sender, ToggledEventArgs e)
    {
        await MapWebView.EvaluateJavaScriptAsync($"setCircleVisible({e.Value.ToString().ToLower()})");
        if (!e.Value)
        {
            await MapWebView.EvaluateJavaScriptAsync("clearMap()"); // Optional, or just rely on setCircleVisible
        }
    }

    private async void OnSearchButtonPressed(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(MapSearchBar.Text)) return;
        await MapWebView.EvaluateJavaScriptAsync($"searchLocation('{MapSearchBar.Text.Replace("'", "\\'")}')");
    }

    private async Task HandleMapClick(double lat, double lng)
    {
        try
        {
            // Toon het paneel weer als er geklikt wordt
            InfoPanel.IsVisible = true;
            ShowPanelButton.IsVisible = false;

            var response = await _httpClient.GetFromJsonAsync<List<ParcelResponseDto>>($"parcels?lat={lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}&lng={lng.ToString(System.Globalization.CultureInfo.InvariantCulture)}");

            if (response != null && response.Count > 0)
            {
                // Zoek het specifieke perceel waar op geklikt is
                _selectedParcel = response.FirstOrDefault(p => IsPointInPolygon(lat, lng, p.Coordinates)) ?? response[0];

                ParcelIdLabel.Text = $"Perceel: {_selectedParcel.ParcelNumber}";
                ParcelSizeLabel.Text = $"Oppervlakte: {(_selectedParcel.AreaInSquareMeters / 10000):F2} hectare";
                
                // Alleen tonen voor jagers
                SaveGroundButton.IsVisible = UserService.CurrentUser?.Role == "Jager";

                string allParcelsJson = JsonSerializer.Serialize(response);
                await MapWebView.EvaluateJavaScriptAsync($"drawParcels({allParcelsJson}, '{_selectedParcel.ParcelNumber}')");
                
                if (CircleToggle.IsToggled)
                {
                    await MapWebView.EvaluateJavaScriptAsync($"drawCircle({lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {lng.ToString(System.Globalization.CultureInfo.InvariantCulture)}, 300)");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fout", $"Fout bij ophalen: {ex.Message}", "OK");
        }
    }

    private bool IsPointInPolygon(double lat, double lng, List<CoordinateDto> polygon)
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

    private async void OnDeselectClicked(object? sender, EventArgs e)
    {
        _selectedParcel = null;
        ParcelIdLabel.Text = "Klik op de kaart om een perceel te selecteren";
        ParcelSizeLabel.Text = "Grootte: --";
        SaveGroundButton.IsVisible = false;
        await MapWebView.EvaluateJavaScriptAsync("deselectParcel()");
    }

    private async void OnRefreshClicked(object? sender, EventArgs e)
    {
        await LoadHuntingGrounds();
        await LoadRegistrations();
        await LoadDamageReports();
        await DisplayAlertAsync("Ververst", "Kaartgegevens zijn opnieuw opgehaald.", "OK");
    }

    private void OnHidePanelClicked(object? sender, EventArgs e)
    {
        InfoPanel.IsVisible = false;
        ShowPanelButton.IsVisible = true;
    }

    private void OnShowPanelClicked(object? sender, EventArgs e)
    {
        InfoPanel.IsVisible = true;
        ShowPanelButton.IsVisible = false;
    }

    private async void OnSaveHuntingGroundClicked(object? sender, EventArgs e)
    {
        if (_selectedParcel == null || UserService.CurrentUser == null || UserService.CurrentUser.Role != "Jager") return;

        var ground = new 
        {
            Name = $"Veld {_selectedParcel.ParcelNumber}",
            TotalAreaInHectares = _selectedParcel.AreaInSquareMeters / 10000,
            PolygonCoordinatesJson = JsonSerializer.Serialize(_selectedParcel.Coordinates),
            UserId = UserService.CurrentUser.Id
        };

        var response = await _httpClient.PostAsJsonAsync("huntinggrounds", ground);
        if (response.IsSuccessStatusCode)
        {
            await DisplayAlertAsync("Succes", "Jachtveld succesvol opgeslagen!", "OK");
            SaveGroundButton.IsVisible = false;
            await LoadHuntingGrounds();
        }
    }
}

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

public class HuntingGroundDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double TotalAreaInHectares { get; set; }
    public string PolygonCoordinatesJson { get; set; } = string.Empty;
    public int UserId { get; set; }
}