using System.Net.Http.Json;
using FaunaConnect2.App.Models;

namespace FaunaConnect2.App;

public partial class MainPage : ContentPage
{
    // De HttpClient die de netwerkoproepen doet
    private readonly HttpClient _httpClient;

    public MainPage()
    {
        InitializeComponent();
        
        _httpClient = new HttpClient();
        
        // Schakel over naar het juiste IP-adres op basis van het platform waar de app op draait!
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            // 10.0.2.2 is het 'loopback' adres voor de Android emulator naar jouw PC
            _httpClient.BaseAddress = new Uri("http://10.0.2.2:5282/api/"); // Pas de poort aan naar jouw HTTP poort!
        }
        else
        {
            // Windows of Mac kan gewoon localhost gebruiken
            _httpClient.BaseAddress = new Uri("http://localhost:5282/api/"); // Pas de poort aan naar jouw HTTP poort!
        }
    }

    // Dit gebeurt er als het scherm opent
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadRegistrationsAsync();
    }

    // Dit gebeurt er als je op de 'Ververs' knop klikt
    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await LoadRegistrationsAsync();
    }

    // De methode die écht met de API praat
    private async Task LoadRegistrationsAsync()
    {
        try
        {
            // Haal de data op en zet het direct om naar een lijst met onze Models
            var registrations = await _httpClient.GetFromJsonAsync<List<Registration>>("registrations");

            // Koppel de lijst aan de UI (de CollectionView in de XAML)
            RegistrationsListView.ItemsSource = registrations;
        }
        catch (Exception ex)
        {
            // Als de API offline is, crash de app dan niet, maar laat een nette foutmelding zien!
            await DisplayAlert("Fout", $"Kan geen gegevens ophalen van de backend: {ex.Message}", "OK");
        }
    }
}