using FaunaConnect2.App.Services;

namespace FaunaConnect2.App;

public partial class AppShell : Shell
{
    private readonly IUserService _userService;

    public AppShell(IUserService userService)
    {
        InitializeComponent();
        _userService = userService;
        
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
        var role = _userService.CurrentUser?.Role;

        MapTab.IsVisible = true;

        AdminTab.IsVisible = role == "Admin";
    }
}