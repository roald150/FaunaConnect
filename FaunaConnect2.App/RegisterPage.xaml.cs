using FaunaConnect2.App.ViewModels;

namespace FaunaConnect2.App;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}