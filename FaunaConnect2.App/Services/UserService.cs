using FaunaConnect2.App.Models;

namespace FaunaConnect2.App.Services;

public static class UserService
{
    public static User? CurrentUser { get; set; }
    
    public static string BaseUrl => DeviceInfo.Platform == DevicePlatform.Android 
        ? "http://10.0.2.2:5282/api/" 
        : "http://127.0.0.1:5282/api/";
}