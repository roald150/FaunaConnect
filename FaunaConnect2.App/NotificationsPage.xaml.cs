using System.Net.Http.Json;
using FaunaConnect2.App.Models;
using FaunaConnect2.App.Services;

namespace FaunaConnect2.App;

public partial class NotificationsPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private List<NotificationItem> _allItems = new();

    public NotificationsPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient { BaseAddress = new Uri(UserService.BaseUrl) };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadNotifications();
    }

    private async void OnRefreshRequested(object? sender, EventArgs e)
    {
        await LoadNotifications();
        NotificationsRefresh.IsRefreshing = false;
    }

    private async Task LoadNotifications()
    {
        try
        {
            var registrations = await _httpClient.GetFromJsonAsync<List<Registration>>("registrations");
            var damageReports = await _httpClient.GetFromJsonAsync<List<DamageReportDto>>("damagereports");

            _allItems.Clear();

            if (registrations != null)
            {
                foreach (var reg in registrations)
                {
                    _allItems.Add(new NotificationItem
                    {
                        Icon = "🦌",
                        Title = "Waarneming",
                        Description = $"{reg.AnimalName} gespot",
                        DateTime = reg.DateTime,
                        Type = "Wildlife",
                        Color = Color.FromArgb("#2B5B2B")
                    });
                }
            }

            if (damageReports != null)
            {
                foreach (var report in damageReports)
                {
                    _allItems.Add(new NotificationItem
                    {
                        Icon = "⚠️",
                        Title = "Wildschade",
                        Description = report.Description ?? "Geen omschrijving",
                        DateTime = report.Timestamp,
                        Type = "Damage",
                        Color = Color.FromArgb("#B22222")
                    });
                }
            }

            _allItems = _allItems.OrderByDescending(x => x.DateTime).ToList();
            NotificationsListView.ItemsSource = _allItems;
        }
        catch { }
    }

    private void OnFilterAllClicked(object? sender, EventArgs e) => NotificationsListView.ItemsSource = _allItems;
    private void OnFilterDamageClicked(object? sender, EventArgs e) => NotificationsListView.ItemsSource = _allItems.Where(x => x.Type == "Damage").ToList();
    private void OnFilterWildlifeClicked(object? sender, EventArgs e) => NotificationsListView.ItemsSource = _allItems.Where(x => x.Type == "Wildlife").ToList();

    public class NotificationItem
    {
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public string Type { get; set; } = string.Empty;
        public Color Color { get; set; } = Colors.Black;
    }
}