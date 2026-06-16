using FaunaConnect2.App.Services;

namespace FaunaConnect2.App;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new NavigationPage(_serviceProvider.GetRequiredService<LoginPage>()));
    }
}
