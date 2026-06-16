using FaunaConnect2.App.Services;
using FaunaConnect2.App.ViewModels;
using Microsoft.Extensions.Logging;

namespace FaunaConnect2.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiMaps()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Services
        builder.Services.AddSingleton<IDeviceService, DeviceService>();
        builder.Services.AddSingleton<SpatialService>();
        builder.Services.AddSingleton<LocalDatabaseService>();
        builder.Services.AddSingleton<IUserService, UserService>();

        // ViewModels
        builder.Services.AddTransient<RegistrationViewModel>();
        builder.Services.AddTransient<RegistrationsViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<MapViewModel>();
        builder.Services.AddTransient<DamageReportViewModel>();
        builder.Services.AddTransient<ChatViewModel>();
        builder.Services.AddTransient<NotificationsViewModel>();
        builder.Services.AddTransient<AdminViewModel>();

        // Pages
        builder.Services.AddTransient<NewRegistrationPage>();
        builder.Services.AddTransient<RegistrationsPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<MapPage>();
        builder.Services.AddTransient<DamageReportPage>();
        builder.Services.AddTransient<ChatPage>();
        builder.Services.AddTransient<NotificationsPage>();
        builder.Services.AddTransient<AdminPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}