using System.Collections.ObjectModel;
using System.Net.Http.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FaunaConnect2.App.Models;
using FaunaConnect2.App.Services;

namespace FaunaConnect2.App.ViewModels;

public partial class NotificationsViewModel(IUserService userService) : BaseViewModel
{
    private readonly IUserService _userService = userService;
    private List<NotificationItem> _allItems = [];

    [ObservableProperty]
    private ObservableCollection<NotificationItem> _filteredItems = [];

    public async Task Initialize()
    {
        await LoadNotifications();
    }

    [RelayCommand]
    private async Task LoadNotifications()
    {
        IsBusy = true;
        try
        {
            var client = await _userService.GetAuthenticatedClient();
            var registrations = await client.GetFromJsonAsync<List<Registration>>("registrations");
            var damageReports = await client.GetFromJsonAsync<List<DamageReportDto>>("damagereports");

            _allItems.Clear();

            if (registrations != null)
            {
                foreach (var reg in registrations)
                {
                    _allItems.Add(new NotificationItem
                    {
                        Icon = "🦌",
                        Title = "Sighting",
                        Description = $"{reg.AnimalName} spotted",
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
                        Title = "Wildlife Damage",
                        Description = report.Description ?? "No description",
                        DateTime = report.Timestamp,
                        Type = "Damage",
                        Color = Color.FromArgb("#B22222")
                    });
                }
            }

            _allItems = [.. _allItems.OrderByDescending(x => x.DateTime)];
            ShowAll();
        }
        catch { }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ShowAll() => FilteredItems = new ObservableCollection<NotificationItem>(_allItems);

    [RelayCommand]
    private void ShowDamage() => FilteredItems = new ObservableCollection<NotificationItem>(_allItems.Where(x => x.Type == "Damage"));

    [RelayCommand]
    private void ShowWildlife() => FilteredItems = new ObservableCollection<NotificationItem>(_allItems.Where(x => x.Type == "Wildlife"));

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
