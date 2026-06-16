using FaunaConnect2.App.Services;

namespace FaunaConnect2.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        RegisterRoutes();
        ConfigureTabs();
    }

    private void RegisterRoutes()
    {
        Routing.RegisterRoute(nameof(NewRegistrationPage), typeof(NewRegistrationPage));
        Routing.RegisterRoute(nameof(DamageReportPage), typeof(DamageReportPage));
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
    }

    private void ConfigureTabs()
    {
        var role = UserService.CurrentUser?.Role;

        // Everyone can see the map
        MapTab.IsVisible = true;

        // Only admin can see admin tab
        AdminTab.IsVisible = role == "Admin";
    }
}