using System.Net.Http.Json;
using FaunaConnect2.App.Models;
using FaunaConnect2.App.Services;
using FaunaConnect2.Api.Helpers;

namespace FaunaConnect2.App;

public partial class MainPage : ContentPage
{
    private readonly HttpClient _httpClient;

    public MainPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient { BaseAddress = new Uri(UserService.BaseUrl) };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (UserService.CurrentUser != null)
        {
            WelcomeLabel.Text = $"Welkom, {UserService.CurrentUser.Name}";
            
            // Beide rollen mogen nu beide acties doen
            bool isJagerOfBoer = UserService.CurrentUser.Role == "Jager" || UserService.CurrentUser.Role == "Boer";
            NewRegistrationButton.IsVisible = isJagerOfBoer;
            ReportDamageButton.IsVisible = isJagerOfBoer;
        }

        await LoadDashboardData();
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        UserService.CurrentUser = null;
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new NavigationPage(new LoginPage());
        }
    }

    private async void OnReportDamageClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new DamageReportPage());
    }

    private async Task LoadDashboardData()
    {
        try
        {
            var weather = await _httpClient.GetFromJsonAsync<WeatherInfo>("weather?lat=51.65&lng=5.05");
            if (weather != null)
            {
                WindLabel.Text = $"{weather.WindSpeed} km/h";
                WindDirLabel.Text = WeatherHelper.GetWindDirection(weather.WindDirection);
            }

            var sun = await _httpClient.GetFromJsonAsync<SunInfo>("sun?lat=51.65&lng=5.05");
            if (sun != null)
            {
                SunriseLabel.Text = $"↑ {sun.Sunrise:HH:mm}";
                SunsetLabel.Text = $"↓ {sun.Sunset:HH:mm}";
            }

            var regs = await _httpClient.GetFromJsonAsync<List<Registration>>("registrations");
            RegistrationsListView.ItemsSource = regs;
        }
        catch (Exception) { /* Foutloos doorgaan voor demo */ }
    }

    private async void OnNewRegistrationClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new NewRegistrationPage());
    }

    public class WeatherInfo { public double WindSpeed { get; set; } public double WindDirection { get; set; } }
    public class SunInfo { public DateTime Sunrise { get; set; } public DateTime Sunset { get; set; } }
}