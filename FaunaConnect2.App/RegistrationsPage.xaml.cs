using System.Net.Http.Json;
using FaunaConnect2.App.Models;
using FaunaConnect2.App.Services;

namespace FaunaConnect2.App;

public partial class RegistrationsPage : ContentPage
{
    private readonly HttpClient _httpClient;

    public RegistrationsPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient { BaseAddress = new Uri(UserService.BaseUrl) };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadRegistrations();
    }

    private async Task LoadRegistrations()
    {
        try
        {
            var regs = await _httpClient.GetFromJsonAsync<List<Registration>>("registrations");
            RegistrationsListView.ItemsSource = regs;
        }
        catch { }
    }
}