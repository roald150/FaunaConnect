using FaunaConnect2.Api.Resources;

namespace FaunaConnect2.Api.Services;

public interface IUserService
{
    Task<List<UserResource>> GetAllAsync();
    Task<UserResource?> GetByIdAsync(int id);
    Task<UserResource> RegisterAsync(Models.User user);
    Task<LoginResponse?> LoginAsync(string email, string password);
    Task UpdateAsync(int id, Models.User updatedUser);
    Task DeleteAsync(int id);
}

public class LoginResponse
{
    public string Token { get; set; } = "";
    public UserResource User { get; set; } = null!;
}
