namespace FaunaConnect2.App.Services;

public class DeviceService : IDeviceService
{
    public async Task<Location?> GetCurrentLocationAsync()
    {
        try
        {
            if (!await RequestLocationPermissionAsync())
                return null;

            return await Geolocation.Default.GetLastKnownLocationAsync() ?? 
                   await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<FileResult?> TakePhotoAsync()
    {
        try
        {
            if (!await RequestCameraPermissionAsync())
                return null;

            if (MediaPicker.Default.IsCaptureSupported)
            {
                return await MediaPicker.Default.CapturePhotoAsync();
            }
            
            await DisplayAlertAsync("Not Supported", "Camera is not supported on this device.", "OK");
            return null;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Camera error: {ex.Message}", "OK");
            return null;
        }
    }

    public async Task<bool> RequestCameraPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Camera>();
        }

        if (status != PermissionStatus.Granted)
        {
            await DisplayAlertAsync("Access Denied", "The app needs camera access to take photos.", "OK");
            return false;
        }

        return true;
    }

    public async Task<bool> RequestLocationPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        if (status != PermissionStatus.Granted)
        {
            await DisplayAlertAsync("Access Denied", "The app needs location access to pinpoint sightings.", "OK");
            return false;
        }

        return true;
    }

    public async Task DisplayAlertAsync(string title, string message, string cancel)
    {
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }
    }
}
