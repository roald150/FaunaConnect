using System.Net.Http.Json;
using FaunaConnect2.App.Models;
using FaunaConnect2.App.Services;

namespace FaunaConnect2.App;

public partial class LoginPage : ContentPage
{
    private readonly HttpClient _httpClient;

    public LoginPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient { BaseAddress = new Uri(UserService.BaseUrl) };
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        var email = EmailEntry.Text?.Trim();
        var password = PasswordEntry.Text;

        var loginData = new { Email = email, Password = password };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("users/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<User>();
                UserService.CurrentUser = user;
                
                // Schakel over naar de volledige app navigatie (AppShell)
                if (Application.Current?.Windows.Count > 0)
                {
                    Application.Current.Windows[0].Page = new AppShell();
                }
            }
            else
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                await DisplayAlertAsync("Fout", string.IsNullOrEmpty(errorMsg) ? "Inloggen mislukt." : errorMsg, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fout", $"Kan geen verbinding maken met de API: {ex.Message}", "OK");
        }
    }

    private async void OnGoToRegisterClicked(object? sender, EventArgs e)
    {
        // Navigeer naar de registratiepagina
        await Navigation.PushAsync(new RegisterPage());
    }
}