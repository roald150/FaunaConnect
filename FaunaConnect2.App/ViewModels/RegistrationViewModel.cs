using System.Collections.ObjectModel;
using System.Net.Http.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FaunaConnect2.App.Models;
using FaunaConnect2.App.Services;

namespace FaunaConnect2.App.ViewModels;

public partial class RegistrationViewModel(IDeviceService deviceService) : BaseViewModel
{
    private readonly IDeviceService _deviceService = deviceService;

    [ObservableProperty]
    private string _selectedSpecies = string.Empty;

    [ObservableProperty]
    private string _locationText = "Location: not yet determined";

    [ObservableProperty]
    private ImageSource? _capturedImageSource;

    [ObservableProperty]
    private bool _isImageVisible;

    [ObservableProperty]
    private Location? _currentLocation;

    [ObservableProperty]
    private ObservableCollection<string> _speciesList = [];

    private FileResult? _photo;

    public void Initialize()
    {
        LoadSpecies();
        InitializeLocation();
    }

    partial void OnCurrentLocationChanged(Location? value)
    {
        if (value != null)
        {
            LocationText = $"Location: {value.Latitude:F5}, {value.Longitude:F5}";
        }
    }

    private async void InitializeLocation()
    {
        CurrentLocation = await _deviceService.GetCurrentLocationAsync();
    }

    private async void LoadSpecies()
    {
        try
        {
            var client = await UserService.GetAuthenticatedClient();
            var species = await client.GetFromJsonAsync<List<AnimalSpeciesDto>>("animalspecies");
            if (species is { Count: > 0 })
            {
                SpeciesList = new ObservableCollection<string>(species.Select(s => s.Name));
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
        SpeciesList = ["Roe Deer", "Wild Boar", "Red Deer", "Fallow Deer", "Fox", "Goose"];
    }

    [RelayCommand]
    private async Task TakePhoto()
    {
        _photo = await _deviceService.TakePhotoAsync();
        if (_photo != null)
        {
            // Refresh location when photo is taken
            var loc = await _deviceService.GetCurrentLocationAsync();
            if (loc != null)
            {
                CurrentLocation = loc;
            }

            var stream = await _photo.OpenReadAsync();
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            CapturedImageSource = ImageSource.FromStream(() => memoryStream);
            IsImageVisible = true;
        }
    }

    [RelayCommand]
    private void PickLocation()
    {
        OnPickLocationRequested?.Invoke(this, CurrentLocation);
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrEmpty(SelectedSpecies))
        {
            await _deviceService.DisplayAlertAsync("Warning", "Please select an animal species first.", "OK");
            return;
        }

        IsBusy = true;
        try
        {
            CurrentLocation ??= await _deviceService.GetCurrentLocationAsync();

            var reg = new Registration
            {
                AnimalName = SelectedSpecies,
                Latitude = CurrentLocation?.Latitude ?? 0,
                Longitude = CurrentLocation?.Longitude ?? 0,
                UserId = UserService.CurrentUser?.Id ?? 0,
                IsSynced = false
            };

            try
            {
                var client = await UserService.GetAuthenticatedClient();
                var response = await client.PostAsJsonAsync("registrations", reg);
                if (response.IsSuccessStatusCode)
                {
                    reg.IsSynced = true;
                    await _deviceService.DisplayAlertAsync("Success", "Registration saved online!", "OK");
                }
                else
                {
                    await SaveLocally(reg, "API error. Saved locally for later sync.");
                }
            }
            catch (Exception)
            {
                await SaveLocally(reg, "No internet. Saved locally for later sync.");
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await _deviceService.DisplayAlertAsync("Error", $"Save failed: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveLocally(Registration reg, string message)
    {
        var localDb = new LocalDatabaseService();
        await localDb.SaveRegistrationAsync(reg);
        await _deviceService.DisplayAlertAsync("Offline", message, "OK");
    }

    public event EventHandler<Location?>? OnPickLocationRequested;

    public class AnimalSpeciesDto { public string Name { get; set; } = ""; }
}
