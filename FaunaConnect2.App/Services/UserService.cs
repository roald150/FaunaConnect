using FaunaConnect2.App.Models;
using System.Net.Http.Headers;

namespace FaunaConnect2.App.Services;

public interface IUserService
{
    User? CurrentUser { get; set; }
    string BaseUrl { get; }
    Task SaveToken(string token);
    Task<string?> GetToken();
    void Logout();
    Task<HttpClient> GetAuthenticatedClient();
}

public class UserService : IUserService
{
    private const string AuthTokenKey = "auth_token";
    public User? CurrentUser { get; set; }
    
    public string BaseUrl => DeviceInfo.Platform == DevicePlatform.Android 
        ? "http://10.0.2.2:5282/api/" 
        : "http://127.0.0.1:5282/api/";

    public async Task SaveToken(string token)
    {
        await SecureStorage.Default.SetAsync(AuthTokenKey, token);
    }

    public async Task<string?> GetToken()
    {
        return await SecureStorage.Default.GetAsync(AuthTokenKey);
    }

    public void Logout()
    {
        SecureStorage.Default.Remove(AuthTokenKey);
        CurrentUser = null;
    }

    public async Task<HttpClient> GetAuthenticatedClient()
    {
        var client = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        var token = await GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return client;
    }
}
