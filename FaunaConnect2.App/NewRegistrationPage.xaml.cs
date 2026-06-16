using FaunaConnect2.App.ViewModels;

namespace FaunaConnect2.App;

public partial class NewRegistrationPage : ContentPage
{
    private readonly RegistrationViewModel _viewModel;

    public NewRegistrationPage(RegistrationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;

        _viewModel.OnPickLocationRequested += async (s, location) => {
            var pickerPage = new LocationPickerPage(location);
            await Navigation.PushModalAsync(pickerPage);

            // Wait for modal to close
            pickerPage.Disappearing += (sender, args) => {
                if (pickerPage.SelectedLocation != null)
                {
                    _viewModel.CurrentLocation = pickerPage.SelectedLocation;
                }
            };
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Initialize();
    }
}