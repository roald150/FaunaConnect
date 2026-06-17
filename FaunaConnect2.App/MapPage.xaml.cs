using System.Text.Json;
using System.Globalization;
using System.Linq;
using FaunaConnect2.App.ViewModels;

namespace FaunaConnect2.App;

public partial class MapPage : ContentPage
{
    private readonly MapViewModel _viewModel;

    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;

        // Subscribe to ViewModel events for JS interop
        _viewModel.OnHuntingGroundsLoaded += async (s, grounds) => await MainThread.InvokeOnMainThreadAsync(async () => await UpdateMap("drawHuntingGrounds", grounds));
        _viewModel.OnRegistrationsLoaded += async (s, regs) => await MainThread.InvokeOnMainThreadAsync(async () => await UpdateMap("drawRegistrations", regs));
        _viewModel.OnDamageReportsLoaded += async (s, reports) => await MainThread.InvokeOnMainThreadAsync(async () => await UpdateMap("drawDamageReports", reports));
        _viewModel.OnParcelsLoaded += async (s, data) => {
            await MainThread.InvokeOnMainThreadAsync(async () => {
                try 
                {
                    string json = JsonSerializer.Serialize(data.Parcels);
                    await MapWebView.EvaluateJavaScriptAsync($"drawParcels({json}, '{data.SelectedId}')");
                    if (_viewModel.IsCircleVisible)
                    {
                        await MapWebView.EvaluateJavaScriptAsync($"drawCircle({data.Lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {data.Lng.ToString(System.Globalization.CultureInfo.InvariantCulture)}, 300)");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error updating parcels: {ex.Message}");
                }
            });
        };
        _viewModel.OnParcelsInBoundsLoaded += async (s, parcels) => await MainThread.InvokeOnMainThreadAsync(async () => await UpdateMap("drawParcels", parcels, ", null, true"));
        _viewModel.OnParcelDeselected += async (s, e) => await MainThread.InvokeOnMainThreadAsync(async () => await MapWebView.EvaluateJavaScriptAsync("deselectParcel()"));
        _viewModel.OnSearchRequested += async (s, text) => await MainThread.InvokeOnMainThreadAsync(async () => await MapWebView.EvaluateJavaScriptAsync($"searchLocation('{text?.Replace("'", "\\'")}')"));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAllData();
    }

    private async Task UpdateMap(string functionName, object data, string extraArgs = "")
    {
        try 
        {
            string json = JsonSerializer.Serialize(data);
            await MapWebView.EvaluateJavaScriptAsync($"{functionName}({json}{extraArgs})");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error calling JS function {functionName}: {ex.Message}");
        }
    }

    private async void OnWebViewNavigating(object? sender, WebNavigatingEventArgs e)
    {
        if (e.Url.Contains("invoke://"))
        {
            e.Cancel = true;
            try 
            {
                var uri = new Uri(e.Url);
                var queryParams = uri.Query.TrimStart('?')
                    .Split('&', StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Split('='))
                    .ToDictionary(p => p[0], p => p.Length > 1 ? Uri.UnescapeDataString(p[1]) : "");

                if (e.Url.Contains("mapclick"))
                {
                    if (queryParams.TryGetValue("lat", out var latStr) && double.TryParse(latStr, System.Globalization.CultureInfo.InvariantCulture, out double lat) &&
                        queryParams.TryGetValue("lng", out var lngStr) && double.TryParse(lngStr, System.Globalization.CultureInfo.InvariantCulture, out double lng))
                    {
                        InfoPanel.IsVisible = true;
                        ShowPanelButton.IsVisible = false;
                        await _viewModel.HandleMapClick(lat, lng);
                    }
                }
                else if (e.Url.Contains("mapbounds"))
                {
                    if (double.TryParse(queryParams.GetValueOrDefault("swLat"), System.Globalization.CultureInfo.InvariantCulture, out double swLat) && 
                        double.TryParse(queryParams.GetValueOrDefault("swLng"), System.Globalization.CultureInfo.InvariantCulture, out double swLng) &&
                        double.TryParse(queryParams.GetValueOrDefault("neLat"), System.Globalization.CultureInfo.InvariantCulture, out double neLat) &&
                        double.TryParse(queryParams.GetValueOrDefault("neLng"), System.Globalization.CultureInfo.InvariantCulture, out double neLng))
                    {
                        await _viewModel.LoadParcelsInBounds(swLat, swLng, neLat, neLng);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation interop error: {ex.Message}");
            }
        }
    }

    private async void OnCircleToggleToggled(object? sender, ToggledEventArgs e)
    {
        await MapWebView.EvaluateJavaScriptAsync($"setCircleVisible({e.Value.ToString().ToLower()})");
    }

    private async void OnDamageToggleToggled(object? sender, ToggledEventArgs e)
    {
        await _viewModel.LoadDamageReports();
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
}