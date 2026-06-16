namespace FaunaConnect2.App.Services;

public interface IDeviceService
{
    Task<Location?> GetCurrentLocationAsync();
    Task<FileResult?> TakePhotoAsync();
    Task<bool> RequestCameraPermissionAsync();
    Task<bool> RequestLocationPermissionAsync();
    Task DisplayAlertAsync(string title, string message, string cancel);
}
