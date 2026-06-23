namespace FaunaConnect2.Api.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
        
    // Role: "Hunter", "Farmer" or "Admin"
    public string Role { get; set; } = "Hunter"; 

    // Relationships
    public List<Registration> Registrations { get; set; } = new();
    public List<HuntingGround> HuntingGrounds { get; set; } = new();
    public List<DamageReport> DamageReports { get; set; } = new();
    

    public int? LinkedHunterId { get; set; }
    public User? LinkedHunter { get; set; }
    

    public List<User> LinkedFarmers { get; set; } = new();
}