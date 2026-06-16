using CommunityToolkit.Mvvm.ComponentModel;

namespace FaunaConnect2.App.ViewModels;

/// <summary>
/// Base class for all ViewModels, leveraging CommunityToolkit.Mvvm.
/// </summary>
public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = string.Empty;
}
