using FaunaConnect2.App.ViewModels;

namespace FaunaConnect2.App;

public partial class DamageReportPage : ContentPage
{
    private readonly DamageReportViewModel _viewModel;

    public DamageReportPage(DamageReportViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;

        _viewModel.OnPickLocationRequested += async (s, location) => {
            var pickerPage = new LocationPickerPage(location);
            await Navigation.PushModalAsync(pickerPage);

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