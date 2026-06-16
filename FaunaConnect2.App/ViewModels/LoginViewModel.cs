using System.Net.Http.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FaunaConnect2.App.Models;
using FaunaConnect2.App.Services;

namespace FaunaConnect2.App.ViewModels;

public partial class LoginViewModel(IUserService userService, IDeviceService deviceService) : BaseViewModel
{
    private readonly IUserService _userService = userService;
    private readonly IDeviceService _deviceService = deviceService;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [RelayCommand]
    private async Task Login()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await _deviceService.DisplayAlertAsync("Warning", "Please enter email and password.", "OK");
            return;
        }

        IsBusy = true;
        try
        {
            using var client = new HttpClient { BaseAddress = new Uri(_userService.BaseUrl) };
            var loginData = new { Email = Email.Trim(), Password = Password };
            var response = await client.PostAsJsonAsync("users/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (loginResponse != null)
                {
                    _userService.CurrentUser = loginResponse.User;
                    await _userService.SaveToken(loginResponse.Token);
                    
                    if (Application.Current != null)
                    {
                        Application.Current.MainPage = new AppShell();
                    }
                }
            }
            else
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                await _deviceService.DisplayAlertAsync("Error", string.IsNullOrEmpty(errorMsg) ? "Login failed." : errorMsg, "OK");
            }
        }
        catch (Exception ex)
        {
            await _deviceService.DisplayAlertAsync("Error", $"Cannot connect to API: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToRegister()
    {
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }
}
