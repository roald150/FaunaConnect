using System.Net.Http.Json;

namespace FaunaConnect2.App;

public partial class RegisterPage : ContentPage
{
    private readonly HttpClient _httpClient;

    public RegisterPage()
    {
        InitializeComponent();
        // Zorg dat de poort matcht met jouw API!
        string baseUri = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5282/api/" : "http://localhost:5282/api/";
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUri) };
        RolePicker.SelectedIndex = 0; // Zet standaard op 'Jager'
    }

    private async void OnRegisterClicked(object? sender, EventArgs e)
    {
        var signUpData = new
        {
            Name = NameEntry.Text?.Trim(),
            Email = EmailEntry.Text?.Trim(),
            PasswordHash = PasswordEntry.Text,
            Role = RolePicker.SelectedItem?.ToString()
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("users/register", signUpData);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlertAsync("Succes", "Account aangemaakt! Je kunt nu inloggen.", "OK");
                await Navigation.PopAsync(); // Ga terug naar het inlogscherm
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                await DisplayAlertAsync("Fout", error, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fout", $"Verbindingsfout: {ex.Message}", "OK");
        }
    }
}