namespace FaunaConnect2.Api.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
        
    // De rol: "Jager", "Boer" of "Admin"
    public string Role { get; set; } = "Jager"; 

    // Relatie: Een jager kan veel waarnemingen registreren
    public List<Registration> Registrations { get; set; } = new();
}