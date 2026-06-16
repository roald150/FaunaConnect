using System.Text.Json;
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
        _viewModel.OnHuntingGroundsLoaded += async (s, grounds) => await UpdateMap("drawHuntingGrounds", grounds);
        _viewModel.OnRegistrationsLoaded += async (s, regs) => await UpdateMap("drawRegistrations", regs);
        _viewModel.OnDamageReportsLoaded += async (s, reports) => await UpdateMap("drawDamageReports", reports);
        _viewModel.OnParcelsLoaded += async (s, data) => {
            string json = JsonSerializer.Serialize(data.Parcels);
            await MapWebView.EvaluateJavaScriptAsync($"drawParcels({json}, '{data.SelectedId}')");
            if (_viewModel.IsCircleVisible)
            {
                await MapWebView.EvaluateJavaScriptAsync($"drawCircle({data.Lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {data.Lng.ToString(System.Globalization.CultureInfo.InvariantCulture)}, 300)");
            }
        };
        _viewModel.OnParcelsInBoundsLoaded += async (s, parcels) => await UpdateMap("drawParcels", parcels, ", null, true");
        _viewModel.OnParcelDeselected += async (s, e) => await MapWebView.EvaluateJavaScriptAsync("deselectParcel()");
        _viewModel.OnSearchRequested += async (s, text) => await MapWebView.EvaluateJavaScriptAsync($"searchLocation('{text.Replace("'", "\\'")}')");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAllData();
    }

    private async Task UpdateMap(string functionName, object data, string extraArgs = "")
    {
        string json = JsonSerializer.Serialize(data);
        await MapWebView.EvaluateJavaScriptAsync($"{functionName}({json}{extraArgs})");
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
                InfoPanel.IsVisible = true;
                ShowPanelButton.IsVisible = false;
                await _viewModel.HandleMapClick(lat, lng);
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
                await _viewModel.LoadParcelsInBounds(swLat, swLng, neLat, neLng);
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