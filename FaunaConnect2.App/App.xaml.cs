using Microsoft.Extensions.DependencyInjection;

namespace FaunaConnect2.App;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new NavigationPage(new LoginPage()));
    }
}