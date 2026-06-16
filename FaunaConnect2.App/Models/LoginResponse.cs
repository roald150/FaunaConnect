using FaunaConnect2.App.Models;

namespace FaunaConnect2.App.Models;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public User User { get; set; } = null!;
}