using System.Collections.ObjectModel;
using System.Net.Http.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FaunaConnect2.App.Models;
using FaunaConnect2.App.Services;

namespace FaunaConnect2.App.ViewModels;

public partial class MainViewModel(IUserService userService, IDeviceService deviceService, IServiceProvider serviceProvider) : BaseViewModel
{
    private readonly IUserService _userService = userService;
    private readonly IDeviceService _deviceService = deviceService;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    [ObservableProperty]
    private string _welcomeMessage = "Welcome";

    [ObservableProperty]
    private string _windText = "-- km/h";

    [ObservableProperty]
    private string _windDirection = "--";

    [ObservableProperty]
    private string _sunriseText = "↑ --:--";

    [ObservableProperty]
    private string _sunsetText = "↓ --:--";

    [ObservableProperty]
    private bool _isActionButtonsVisible;

    [ObservableProperty]
    private ObservableCollection<Registration> _recentRegistrations = [];

    public async Task Initialize()
    {
        if (_userService.CurrentUser != null)
        {
            WelcomeMessage = $"Welcome, {_userService.CurrentUser.Name}";
            IsActionButtonsVisible = _userService.CurrentUser.Role == "Hunter" || _userService.CurrentUser.Role == "Farmer";
        }

        await LoadDashboardData();
    }

    [RelayCommand]
    private async Task LoadDashboardData()
    {
        IsBusy = true;
        try
        {
            var client = await _userService.GetAuthenticatedClient();
            // Using default coordinates for demo (approx location in NL)
            var weather = await client.GetFromJsonAsync<WeatherInfo>("weather?lat=51.65&lng=5.05");
            if (weather != null)
            {
                WindText = $"{weather.WindSpeed} km/h";
                WindDirection = GetWindDirection(weather.WindDirection);
            }

            var sun = await client.GetFromJsonAsync<SunInfo>("sun?lat=51.65&lng=5.05");
            if (sun != null)
            {
                SunriseText = $"↑ {sun.Sunrise:HH:mm}";
                SunsetText = $"↓ {sun.Sunset:HH:mm}";
            }

            var regs = await client.GetFromJsonAsync<List<Registration>>("registrations");
            if (regs != null)
            {
                RecentRegistrations = new ObservableCollection<Registration>(regs.OrderByDescending(r => r.DateTime).Take(10));
            }
        }
        catch (Exception) 
        {
            // Fallback or silent fail for dashboard
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Logout()
    {
        _userService.Logout();
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new NavigationPage(_serviceProvider.GetRequiredService<LoginPage>());
        }
    }

    [RelayCommand]
    private async Task NewRegistration()
    {
        await Shell.Current.GoToAsync(nameof(NewRegistrationPage));
    }

    [RelayCommand]
    private async Task ReportDamage()
    {
        await Shell.Current.GoToAsync(nameof(DamageReportPage));
    }

    private string GetWindDirection(double degrees)
    {
        string[] cardinals = ["N", "NE", "E", "SE", "S", "SW", "W", "NW", "N"];
        return cardinals[(int)Math.Round(((double)degrees % 360) / 45)];
    }

    public class WeatherInfo { public double WindSpeed { get; set; } public double WindDirection { get; set; } }
    public class SunInfo { public DateTime Sunrise { get; set; } public DateTime Sunset { get; set; } }
}
