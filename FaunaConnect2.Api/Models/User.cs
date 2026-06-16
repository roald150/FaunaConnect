namespace FaunaConnect2.Api.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
        
    // De rol: "Jager", "Boer" of "Admin"
    public string Role { get; set; } = "Jager"; 

    // Relaties
    public List<Registration> Registrations { get; set; } = new();
    public List<HuntingGround> HuntingGrounds { get; set; } = new();
    public List<DamageReport> DamageReports { get; set; } = new();
    
    // De jager-boer koppeling:
    // Als de rol 'Boer' is, kan hij gekoppeld zijn aan een Jager
    public int? LinkedJagerId { get; set; }
    public User? LinkedJager { get; set; }
    
    // Een jager kan meerdere boeren hebben
    public List<User> LinkedFarmers { get; set; } = new();
}