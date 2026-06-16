using System.Net.Http.Json;
using FaunaConnect2.App.Models;
using FaunaConnect2.App.Services;

namespace FaunaConnect2.App;

public partial class NewRegistrationPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private FileResult? _photo;
    private Location? _selectedLocation;

    public NewRegistrationPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient { BaseAddress = new Uri(UserService.BaseUrl) };
        LoadSpecies();
        InitializeLocation();
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

    private async void LoadSpecies()
    {
        try
        {
            var species = await _httpClient.GetFromJsonAsync<List<AnimalSpeciesDto>>("animalspecies");
            if (species != null && species.Count > 0)
            {
                SpeciesPicker.ItemsSource = species.Select(s => s.Name).ToList();
            }
            else
            {
                LoadFallbackSpecies();
            }
        }
        catch 
        { 
            LoadFallbackSpecies();
        }
    }

    private void LoadFallbackSpecies()
    {
        SpeciesPicker.ItemsSource = new List<string> { "Ree", "Wild zwijn", "Edelhert", "Damhert", "Vos", "Gans" };
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
                _photo = await MediaPicker.Default.CapturePhotoAsync();
                if (_photo != null)
                {
                    // Update location from photo metadata if available (standard MediaPicker doesn't always provide it)
                    // But we can try to get current location again when photo is taken
                    var loc = await Geolocation.Default.GetLastKnownLocationAsync() ?? 
                              await Geolocation.Default.GetLocationAsync();
                    if (loc != null)
                    {
                        _selectedLocation = loc;
                        UpdateLocationLabel();
                    }

                    var stream = await _photo.OpenReadAsync();
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

        // Wait for modal to close
        pickerPage.Disappearing += (s, args) => {
            if (pickerPage.SelectedLocation != null)
            {
                _selectedLocation = pickerPage.SelectedLocation;
                UpdateLocationLabel();
            }
        };
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (SpeciesPicker.SelectedIndex == -1)
        {
            await DisplayAlertAsync("Let op", "Selecteer eerst een diersoort.", "OK");
            return;
        }

        try
        {
            if (_selectedLocation == null)
            {
                _selectedLocation = await Geolocation.Default.GetLastKnownLocationAsync() ?? 
                                   await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));
            }

            var reg = new Registration
            {
                AnimalName = SpeciesPicker.SelectedItem.ToString() ?? "Onbekend",
                Latitude = _selectedLocation?.Latitude ?? 0,
                Longitude = _selectedLocation?.Longitude ?? 0,
                UserId = UserService.CurrentUser?.Id ?? 0
            };

            var response = await _httpClient.PostAsJsonAsync("registrations", reg);
            if (response.IsSuccessStatusCode)
            {
                await DisplayAlertAsync("Succes", "Registratie opgeslagen!", "OK");
                await Navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fout", $"Opslaan mislukt: {ex.Message}", "OK");
        }
    }

    public class AnimalSpeciesDto { public string Name { get; set; } = ""; }
}