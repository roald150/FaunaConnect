using FaunaConnect2.App.Services;

namespace FaunaConnect2.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        ConfigureTabs();
    }

    private void ConfigureTabs()
    {
        var role = UserService.CurrentUser?.Role;

        // Iedereen mag de kaart zien
        MapTab.IsVisible = true;

        // Alleen admin mag beheer zien
        AdminTab.IsVisible = role == "Admin";
    }
}