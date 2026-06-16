using FaunaConnect2.App.ViewModels;

namespace FaunaConnect2.App;

public partial class RegistrationsPage : ContentPage
{
    private readonly RegistrationsViewModel _viewModel;

    public RegistrationsPage(RegistrationsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.Initialize();
    }
}
