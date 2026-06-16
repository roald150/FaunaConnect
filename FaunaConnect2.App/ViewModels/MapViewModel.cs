using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FaunaConnect2.App.Models;
using FaunaConnect2.App.Services;

namespace FaunaConnect2.App.ViewModels;

public partial class MapViewModel(IUserService userService, IDeviceService deviceService, SpatialService spatialService) : BaseViewModel
{
    private readonly IUserService _userService = userService;
    private readonly IDeviceService _deviceService = deviceService;
    private readonly SpatialService _spatialService = spatialService;

    [ObservableProperty]
    private ParcelResponseDto? _selectedParcel;

    [ObservableProperty]
    private bool _isCircleVisible;

    [ObservableProperty]
    private bool _isDamageVisible = true;

    [ObservableProperty]
    private string _parcelIdText = "Click on the map to select a parcel";

    [ObservableProperty]
    private string _parcelSizeText = "Size: --";

    [ObservableProperty]
    private bool _isSaveGroundVisible;

    [ObservableProperty]
    private string _searchText = string.Empty;

    partial void OnSelectedParcelChanged(ParcelResponseDto? value)
    {
        UpdateParcelInfo();
        IsSaveGroundVisible = value != null && _userService.CurrentUser?.Role == "Hunter";
    }

    [RelayCommand]
    public async Task LoadAllData()
    {
        IsBusy = true;
        try
        {
            await Task.WhenAll(
                LoadHuntingGrounds(),
                LoadRegistrations(),
                LoadDamageReports()
            );
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task LoadHuntingGrounds()
    {
        try
        {
            var client = await _userService.GetAuthenticatedClient();
            string url = "huntinggrounds";
            if (_userService.CurrentUser != null && _userService.CurrentUser.Role == "Hunter")
            {
                url += $"?userId={_userService.CurrentUser.Id}";
            }
            
            var grounds = await client.GetFromJsonAsync<List<HuntingGroundDto>>(url);
            if (grounds != null)
            {
                OnHuntingGroundsLoaded?.Invoke(this, grounds);

                double totalSize = grounds.Sum(g => g.TotalAreaInHectares);
                ParcelSizeText = $"Total Hunting Ground: {totalSize:F2} hectares";
            }
        }
        catch { }
    }

    [RelayCommand]
    public async Task LoadRegistrations()
    {
        try
        {
            var client = await _userService.GetAuthenticatedClient();
            var registrations = await client.GetFromJsonAsync<List<Registration>>("registrations");
            if (registrations != null)
            {
                OnRegistrationsLoaded?.Invoke(this, registrations);
            }
        }
        catch { }
    }

    [RelayCommand]
    public async Task LoadDamageReports()
    {
        if (!IsDamageVisible)
        {
            OnDamageReportsLoaded?.Invoke(this, []);
            return;
        }

        try
        {
            var client = await _userService.GetAuthenticatedClient();
            var reports = await client.GetFromJsonAsync<List<DamageReportDto>>("damagereports");
            if (reports != null)
            {
                OnDamageReportsLoaded?.Invoke(this, reports);
            }
        }
        catch { }
    }

    public async Task HandleMapClick(double lat, double lng)
    {
        try
        {
            var client = await _userService.GetAuthenticatedClient();
            var response = await client.GetFromJsonAsync<List<ParcelResponseDto>>($"parcels?lat={lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}&lng={lng.ToString(System.Globalization.CultureInfo.InvariantCulture)}");

            if (response is { Count: > 0 })
            {
                SelectedParcel = response.FirstOrDefault(p => _spatialService.IsPointInPolygon(lat, lng, p.Coordinates)) ?? response[0];
                OnParcelsLoaded?.Invoke(this, (response, SelectedParcel.ParcelNumber, lat, lng));
            }
        }
        catch (Exception ex)
        {
            await _deviceService.DisplayAlertAsync("Error", $"Error retrieving data: {ex.Message}", "OK");
        }
    }

    public async Task LoadParcelsInBounds(double swLat, double swLng, double neLat, double neLng)
    {
        try
        {
            IsBusy = true;
            var client = await _userService.GetAuthenticatedClient();
            var url = $"parcels/bbox?minLat={swLat.ToString(System.Globalization.CultureInfo.InvariantCulture)}&minLng={swLng.ToString(System.Globalization.CultureInfo.InvariantCulture)}&maxLat={neLat.ToString(System.Globalization.CultureInfo.InvariantCulture)}&maxLng={neLng.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            var parcels = await client.GetFromJsonAsync<List<ParcelResponseDto>>(url);
            
            if (parcels != null)
            {
                OnParcelsInBoundsLoaded?.Invoke(this, parcels);
            }
        }
        catch { }
        finally
        {
            IsBusy = false;
        }
    }

    private void UpdateParcelInfo()
    {
        if (SelectedParcel != null)
        {
            ParcelIdText = $"Parcel: {SelectedParcel.ParcelNumber}";
            ParcelSizeText = $"Area: {(SelectedParcel.AreaInSquareMeters / 10000):F2} hectares";
        }
        else
        {
            ParcelIdText = "Click on the map to select a parcel";
            ParcelSizeText = "Size: --";
        }
    }

    [RelayCommand]
    private void Deselect()
    {
        SelectedParcel = null;
        OnParcelDeselected?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task SaveHuntingGround()
    {
        if (SelectedParcel == null || _userService.CurrentUser == null || _userService.CurrentUser.Role != "Hunter") return;

        var ground = new 
        {
            Name = $"Ground {SelectedParcel.ParcelNumber}",
            TotalAreaInHectares = SelectedParcel.AreaInSquareMeters / 10000,
            PolygonCoordinatesJson = JsonSerializer.Serialize(SelectedParcel.Coordinates),
            UserId = _userService.CurrentUser.Id
        };

        var client = await _userService.GetAuthenticatedClient();
        var response = await client.PostAsJsonAsync("huntinggrounds", ground);
        if (response.IsSuccessStatusCode)
        {
            await _deviceService.DisplayAlertAsync("Success", "Hunting ground saved successfully!", "OK");
            IsSaveGroundVisible = false;
            await LoadHuntingGrounds();
        }
    }

    [RelayCommand]
    private async Task ReportDamage()
    {
        await Shell.Current.GoToAsync(nameof(DamageReportPage));
    }

    [RelayCommand]
    private void Search()
    {
        if (string.IsNullOrWhiteSpace(SearchText)) return;
        OnSearchRequested?.Invoke(this, SearchText);
    }

    // Events to communicate back to the View for JavaScript interop
    public event EventHandler<List<HuntingGroundDto>>? OnHuntingGroundsLoaded;
    public event EventHandler<List<Registration>>? OnRegistrationsLoaded;
    public event EventHandler<List<DamageReportDto>>? OnDamageReportsLoaded;
    public event EventHandler<(List<ParcelResponseDto> Parcels, string SelectedId, double Lat, double Lng)>? OnParcelsLoaded;
    public event EventHandler<List<ParcelResponseDto>>? OnParcelsInBoundsLoaded;
    public event EventHandler? OnParcelDeselected;
    public event EventHandler<string>? OnSearchRequested;
}

public class ParcelResponseDto
{
    public string ParcelNumber { get; set; } = string.Empty;
    public double AreaInSquareMeters { get; set; }
    public List<CoordinateDto> Coordinates { get; set; } = [];
}

public class HuntingGroundDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double TotalAreaInHectares { get; set; }
    public string PolygonCoordinatesJson { get; set; } = string.Empty;
    public int UserId { get; set; }
}
