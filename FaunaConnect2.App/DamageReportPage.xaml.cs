using System.Net.Http.Json;
using FaunaConnect2.App.Models;
using FaunaConnect2.App.Services;

namespace FaunaConnect2.App;

public partial class DamageReportPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private Location? _selectedLocation;

    public DamageReportPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient { BaseAddress = new Uri(UserService.BaseUrl) };
        InitializeLocation();
        _ = LoadReports();
    }

    private async Task LoadReports()
    {
        try
        {
            var reports = await _httpClient.GetFromJsonAsync<List<DamageReportDto>>("damagereports");
            ReportsListView.ItemsSource = reports;
        }
        catch { }
    }

    private async void InitializeLocation()
    {
        try
        {
            _selectedLocation = await Geolocation.Default.GetLastKnownLocationAsync() ?? 
                                await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));
            UpdateLocationLabel();
        }
        catch { }
    }

    private void UpdateLocationLabel()
    {
        if (_selectedLocation != null)
        {
            LocationLabel.Text = $"Locatie: {_selectedLocation.Latitude:F5}, {_selectedLocation.Longitude:F5}";
        }
    }

    private async void OnTakePhotoClicked(object? sender, EventArgs e)
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }

            if (status != PermissionStatus.Granted)
            {
                await DisplayAlertAsync("Toegang geweigerd", "De app heeft camera-toegang nodig om foto's te maken.", "OK");
                return;
            }

            if (MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null)
                {
                    // Update location when photo is taken
                    var loc = await Geolocation.Default.GetLastKnownLocationAsync() ?? 
                              await Geolocation.Default.GetLocationAsync();
                    if (loc != null)
                    {
                        _selectedLocation = loc;
                        UpdateLocationLabel();
                    }

                    var stream = await photo.OpenReadAsync();
                    var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    CapturedImage.Source = ImageSource.FromStream(() => memoryStream);
                    CapturedImage.IsVisible = true;
                }
            }
            else
            {
                await DisplayAlertAsync("Niet ondersteund", "Camera wordt niet ondersteund op dit apparaat.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fout", $"Camera fout: {ex.Message}", "OK");
        }
    }

    private async void OnPickLocationClicked(object? sender, EventArgs e)
    {
        var pickerPage = new LocationPickerPage(_selectedLocation);
        await Navigation.PushModalAsync(pickerPage);

        pickerPage.Disappearing += (s, args) => {
            if (pickerPage.SelectedLocation != null)
            {
                _selectedLocation = pickerPage.SelectedLocation;
                UpdateLocationLabel();
            }
        };
    }

    private async void OnSendClicked(object? sender, EventArgs e)
    {
        try
        {
            if (_selectedLocation == null)
            {
                _selectedLocation = await Geolocation.Default.GetLastKnownLocationAsync() ?? 
                                   await Geolocation.Default.GetLocationAsync();
            }

            var report = new
            {
                Description = DescriptionEditor.Text,
                Latitude = _selectedLocation?.Latitude ?? 0,
                Longitude = _selectedLocation?.Longitude ?? 0,
                UserId = UserService.CurrentUser?.Id ?? 0
            };

            var response = await _httpClient.PostAsJsonAsync("damagereports", report);
            if (response.IsSuccessStatusCode)
            {
                await DisplayAlertAsync("Gelukt", "Schademelding is verzonden naar de jager.", "OK");
                await Navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fout", ex.Message, "OK");
        }
    }
}