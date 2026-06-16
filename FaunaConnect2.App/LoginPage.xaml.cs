using FaunaConnect2.App.ViewModels;

namespace FaunaConnect2.App;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}