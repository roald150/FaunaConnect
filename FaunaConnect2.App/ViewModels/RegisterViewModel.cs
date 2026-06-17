using System.Collections.ObjectModel;
using System.Net.Http.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FaunaConnect2.App.Services;

namespace FaunaConnect2.App.ViewModels;

public partial class RegisterViewModel(IUserService userService, IDeviceService deviceService) : BaseViewModel
{
    private readonly IUserService _userService = userService;
    private readonly IDeviceService _deviceService = deviceService;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _selectedRole = "Hunter";

    public ObservableCollection<string> Roles { get; } = ["Hunter", "Farmer"];

    [RelayCommand]
    private async Task Register()
    {
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await _deviceService.DisplayAlertAsync("Warning", "Please fill in all fields.", "OK");
            return;
        }

        IsBusy = true;
        try
        {
            var signUpData = new
            {
                Name = Name.Trim(),
                Email = Email.Trim(),
                PasswordHash = Password,
                Role = SelectedRole
            };

            var client = await _userService.GetAuthenticatedClient();
            var response = await client.PostAsJsonAsync("users/register", signUpData);

            if (response.IsSuccessStatusCode)
            {
                await _deviceService.DisplayAlertAsync("Success", "Account created! You can now log in.", "OK");
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                await _deviceService.DisplayAlertAsync("Error", error, "OK");
            }
        }
        catch (Exception ex)
        {
            await _deviceService.DisplayAlertAsync("Error", $"Connection error: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
