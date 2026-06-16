using System.Net.Http.Json;
using FaunaConnect2.App.Models;
using FaunaConnect2.App.Services;

namespace FaunaConnect2.App;

public partial class AdminPage : ContentPage
{
    private readonly HttpClient _httpClient;

    public AdminPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient { BaseAddress = new Uri(UserService.BaseUrl) };
        LoadSpecies();
        LoadUsers();
    }

    private async void LoadSpecies()
    {
        try
        {
            var species = await _httpClient.GetFromJsonAsync<List<SpeciesDto>>("animalspecies");
            SpeciesListView.ItemsSource = species;
        }
        catch { }
    }

    private async void LoadUsers()
    {
        try
        {
            var users = await _httpClient.GetFromJsonAsync<List<User>>("users");
            UsersListView.ItemsSource = users;
        }
        catch { }
    }

    private async void OnAddClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NewSpeciesEntry.Text)) return;

        var response = await _httpClient.PostAsJsonAsync("animalspecies", new { Name = NewSpeciesEntry.Text });
        if (response.IsSuccessStatusCode)
        {
            NewSpeciesEntry.Text = "";
            LoadSpecies();
        }
    }

    private async void OnAddUserClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UserNameEntry.Text) || string.IsNullOrWhiteSpace(UserEmailEntry.Text) || UserRolePicker.SelectedItem == null)
        {
            await DisplayAlertAsync("Fout", "Vul alle velden in.", "OK");
            return;
        }

        var newUser = new User
        {
            Name = UserNameEntry.Text,
            Email = UserEmailEntry.Text,
            Role = UserRolePicker.SelectedItem.ToString() ?? "Jager"
        };

        var response = await _httpClient.PostAsJsonAsync("users/register", newUser);
        if (response.IsSuccessStatusCode)
        {
            UserNameEntry.Text = "";
            UserEmailEntry.Text = "";
            UserRolePicker.SelectedItem = null;
            LoadUsers();
        }
        else
        {
            await DisplayAlertAsync("Fout", "Kon gebruiker niet aanmaken.", "OK");
        }
    }

    private async void OnEditUserClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is User user)
        {
            string action = await DisplayActionSheetAsync($"Gebruiker {user.Name} bewerken", "Annuleren", null, "Naam wijzigen", "Email wijzigen", "Rol wijzigen");

            if (action == "Naam wijzigen")
            {
                string res = await DisplayPromptAsync("Naam", "Nieuwe naam:", initialValue: user.Name);
                if (!string.IsNullOrEmpty(res)) user.Name = res;
            }
            else if (action == "Email wijzigen")
            {
                string res = await DisplayPromptAsync("Email", "Nieuwe email:", initialValue: user.Email);
                if (!string.IsNullOrEmpty(res)) user.Email = res;
            }
            else if (action == "Rol wijzigen")
            {
                string newRole = await DisplayActionSheetAsync("Kies nieuwe rol", "Annuleren", null, "Jager", "Boer", "Admin");
                if (newRole != "Annuleren" && !string.IsNullOrEmpty(newRole))
                {
                    user.Role = newRole;
                }
            }
            else
            {
                return;
            }

            var response = await _httpClient.PutAsJsonAsync($"users/{user.Id}", user);
            if (response.IsSuccessStatusCode)
            {
                LoadUsers();
            }
            else
            {
                await DisplayAlertAsync("Fout", "Kon gebruiker niet bijwerken.", "OK");
            }
        }
    }

    private async void OnDeleteUserClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is User user)
        {
            bool confirm = await DisplayAlertAsync("Bevestigen", $"Weet u zeker dat u {user.Name} wilt verwijderen?", "Ja", "Nee");
            if (confirm)
            {
                var response = await _httpClient.DeleteAsync($"users/{user.Id}");
                if (response.IsSuccessStatusCode)
                {
                    LoadUsers();
                }
            }
        }
    }

    public class SpeciesDto { public string Name { get; set; } = ""; }
}