using System.Collections.ObjectModel;
using System.Net.Http.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FaunaConnect2.App.Models;
using FaunaConnect2.App.Services;

namespace FaunaConnect2.App.ViewModels;

public partial class RegistrationsViewModel(IUserService userService) : BaseViewModel
{
    private readonly IUserService _userService = userService;

    [ObservableProperty]
    private ObservableCollection<Registration> _registrations = [];

    public async Task Initialize()
    {
        await LoadRegistrations();
    }

    [RelayCommand]
    public async Task LoadRegistrations()
    {
        IsBusy = true;
        try
        {
            var client = await _userService.GetAuthenticatedClient();
            var regs = await client.GetFromJsonAsync<List<Registration>>("registrations");
            if (regs != null)
            {
                Registrations = new ObservableCollection<Registration>(regs);
            }
        }
        catch { }
        finally
        {
            IsBusy = false;
        }
    }
}
