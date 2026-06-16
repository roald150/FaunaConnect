namespace FaunaConnect2.App;

public partial class LocationPickerPage : ContentPage
{
    public Location? SelectedLocation { get; private set; }

    public LocationPickerPage(Location? initialLocation = null)
    {
        InitializeComponent();

        if (initialLocation != null)
        {
            SelectedLocation = initialLocation;
            // Wait for WebView to load before setting location
            PickerWebView.Navigated += (s, e) => {
                SetLocationInWebView(initialLocation.Latitude, initialLocation.Longitude);
            };
        }
        else
        {
            _ = MoveToCurrentLocation();
        }
    }

    private async Task MoveToCurrentLocation()
    {
        try
        {
            var location = await Geolocation.Default.GetLastKnownLocationAsync() ?? 
                           await Geolocation.Default.GetLocationAsync();

            if (location != null)
            {
                SetLocationInWebView(location.Latitude, location.Longitude);
            }
        }
        catch { }
    }

    private void OnWebViewNavigating(object? sender, WebNavigatingEventArgs e)
    {
        if (e.Url.StartsWith("invoke://locationselected"))
        {
            e.Cancel = true;
            var uri = new Uri(e.Url);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            if (double.TryParse(query["lat"], out double lat) && double.TryParse(query["lng"], out double lng))
            {
                SelectedLocation = new Location(lat, lng);
                LocationLabel.Text = $"Selected: {lat:F5}, {lng:F5}";
                ConfirmButton.IsEnabled = true;
            }
        }
    }

    private async void SetLocationInWebView(double lat, double lng)
    {
        await PickerWebView.EvaluateJavaScriptAsync($"centerMap({lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {lng.ToString(System.Globalization.CultureInfo.InvariantCulture)}, 16)");
    }

    private async void OnConfirmClicked(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}