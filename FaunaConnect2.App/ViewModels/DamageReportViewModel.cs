using System.Collections.ObjectModel;
using System.Net.Http.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FaunaConnect2.App.Models;
using FaunaConnect2.App.Services;

namespace FaunaConnect2.App.ViewModels;

public partial class DamageReportViewModel(IDeviceService deviceService) : BaseViewModel
{
    private readonly IDeviceService _deviceService = deviceService;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _locationText = "Location: not yet determined";

    [ObservableProperty]
    private ImageSource? _capturedImageSource;

    [ObservableProperty]
    private bool _isImageVisible;

    [ObservableProperty]
    private Location? _currentLocation;

    [ObservableProperty]
    private ObservableCollection<DamageReportDto> _reports = [];

    private FileResult? _photo;

    public void Initialize()
    {
        InitializeLocation();
        _ = LoadReports();
    }

    partial void OnCurrentLocationChanged(Location? value)
    {
        if (value != null)
        {
            LocationText = $"Location: {value.Latitude:F5}, {value.Longitude:F5}";
        }
    }

    private async Task LoadReports()
    {
        try
        {
            var client = await UserService.GetAuthenticatedClient();
            var reports = await client.GetFromJsonAsync<List<DamageReportDto>>("damagereports");
            if (reports != null)
            {
                Reports = new ObservableCollection<DamageReportDto>(reports);
            }
        }
        catch { }
    }

    private async void InitializeLocation()
    {
        CurrentLocation = await _deviceService.GetCurrentLocationAsync();
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
    private async Task Send()
    {
        if (string.IsNullOrWhiteSpace(Description))
        {
            await _deviceService.DisplayAlertAsync("Warning", "Please enter a description of the damage.", "OK");
            return;
        }

        IsBusy = true;
        try
        {
            CurrentLocation ??= await _deviceService.GetCurrentLocationAsync();

            var report = new DamageReportDto
            {
                Description = Description,
                Latitude = CurrentLocation?.Latitude ?? 0,
                Longitude = CurrentLocation?.Longitude ?? 0,
                UserId = UserService.CurrentUser?.Id ?? 0,
                Timestamp = DateTime.Now,
                IsSynced = false
            };

            try
            {
                var client = await UserService.GetAuthenticatedClient();
                var response = await client.PostAsJsonAsync("damagereports", report);
                if (response.IsSuccessStatusCode)
                {
                    report.IsSynced = true;
                    await _deviceService.DisplayAlertAsync("Success", "Damage report has been sent to the hunter.", "OK");
                }
                else
                {
                    await SaveLocally(report, "API error. Saved locally for later sync.");
                }
            }
            catch (Exception)
            {
                await SaveLocally(report, "No internet. Saved locally for later sync.");
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await _deviceService.DisplayAlertAsync("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveLocally(DamageReportDto report, string message)
    {
        var localDb = new LocalDatabaseService();
        await localDb.SaveDamageReportAsync(report);
        await _deviceService.DisplayAlertAsync("Offline", message, "OK");
    }

    public event EventHandler<Location?>? OnPickLocationRequested;
}
