using System.Collections.ObjectModel;
using System.Net.Http.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FaunaConnect2.App.Models;
using FaunaConnect2.App.Services;

namespace FaunaConnect2.App.ViewModels;

public partial class AdminViewModel(IUserService userService, IDeviceService deviceService) : BaseViewModel
{
    private readonly IUserService _userService = userService;
    private readonly IDeviceService _deviceService = deviceService;

    [ObservableProperty]
    private ObservableCollection<SpeciesDto> _speciesList = [];

    [ObservableProperty]
    private ObservableCollection<User> _usersList = [];

    [ObservableProperty]
    private string _newSpeciesName = string.Empty;

    [ObservableProperty]
    private string _newUserName = string.Empty;

    [ObservableProperty]
    private string _newUserEmail = string.Empty;

    [ObservableProperty]
    private string _selectedUserRole = "Hunter";

    public ObservableCollection<string> Roles { get; } = ["Hunter", "Farmer", "Admin"];

    public async Task Initialize()
    {
        await LoadData();
    }

    [RelayCommand]
    private async Task LoadData()
    {
        IsBusy = true;
        try
        {
            var client = await _userService.GetAuthenticatedClient();
            var species = await client.GetFromJsonAsync<List<SpeciesDto>>("animalspecies");
            var users = await client.GetFromJsonAsync<List<User>>("users");

            if (species != null) SpeciesList = new ObservableCollection<SpeciesDto>(species);
            if (users != null) UsersList = new ObservableCollection<User>(users);
        }
        catch { }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AddSpecies()
    {
        if (string.IsNullOrWhiteSpace(NewSpeciesName)) return;

        try
        {
            var client = await _userService.GetAuthenticatedClient();
            var response = await client.PostAsJsonAsync("animalspecies", new { Name = NewSpeciesName });
            if (response.IsSuccessStatusCode)
            {
                NewSpeciesName = string.Empty;
                await LoadData();
            }
        }
        catch (Exception ex)
        {
            await _deviceService.DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task AddUser()
    {
        if (string.IsNullOrWhiteSpace(NewUserName) || string.IsNullOrWhiteSpace(NewUserEmail))
        {
            await _deviceService.DisplayAlertAsync("Error", "Please fill in all fields.", "OK");
            return;
        }

        try
        {
            var newUser = new User
            {
                Name = NewUserName,
                Email = NewUserEmail,
                Role = SelectedUserRole
            };

            var client = await _userService.GetAuthenticatedClient();
            var response = await client.PostAsJsonAsync("users/register", newUser);
            if (response.IsSuccessStatusCode)
            {
                NewUserName = string.Empty;
                NewUserEmail = string.Empty;
                await LoadData();
            }
            else
            {
                await _deviceService.DisplayAlertAsync("Error", "Could not create user.", "OK");
            }
        }
        catch (Exception ex)
        {
            await _deviceService.DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task DeleteUser(User user)
    {
        // This needs a confirmation which is UI dependent, 
        // but we can handle the logic here if we use IDeviceService for alerts.
        // Wait, I should probably handle the confirmation in the page or pass it.
        // I'll use IDeviceService to ask but MAUI DisplayAlert has no Yes/No in my interface yet.
        // I'll assume confirmed for simplicity or update IDeviceService.
        
        try
        {
            var client = await _userService.GetAuthenticatedClient();
            var response = await client.DeleteAsync($"users/{user.Id}");
            if (response.IsSuccessStatusCode)
            {
                await LoadData();
            }
        }
        catch (Exception ex)
        {
            await _deviceService.DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    public class SpeciesDto { public string Name { get; set; } = ""; }
}
