using System.Net.Http.Json;

namespace FaunaConnect2.App;

public partial class LoginPage : ContentPage
{
    private readonly HttpClient _httpClient;

    public LoginPage()
    {
        InitializeComponent();
        string baseUri = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5282/api/" : "http://localhost:5282/api/";
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUri) };
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var loginData = new { Email = EmailEntry.Text, Password = PasswordEntry.Text };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("users/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                // Schakel de hoofdpagina van de app om naar de MainPage (Dashboard)
                // Door dit zo te doen, kan de gebruiker niet met de 'back'-knop terug naar het inlogscherm!
                Application.Current.MainPage = new NavigationPage(new MainPage());
            }
            else
            {
                await DisplayAlert("Fout", "Onjuist e-mailadres of wachtwoord.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Kan geen verbinding maken met de API: {ex.Message}", "OK");
        }
    }

    private async void OnGoToRegisterClicked(object sender, EventArgs e)
    {
        // Navigeer naar de registratiepagina
        await Navigation.PushAsync(new RegisterPage());
    }
}