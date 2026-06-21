namespace FaunaConnect2.Api.Resources;

public class UserResource
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Hunter";
    public int? LinkedHunterId { get; set; }
    public string? LinkedHunterName { get; set; }
    public int RegistrationCount { get; set; }
    public int DamageReportCount { get; set; }
}
