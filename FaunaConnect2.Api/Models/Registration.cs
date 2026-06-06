namespace FaunaConnect2.Api.Models;

public class Registration
{
    public int Id { get; set; }
    public string AnimalName { get; set; } = string.Empty; // Bijv. "Ree" 
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime DateTime { get; set; } = DateTime.Now;
    public string? PhotoUrl { get; set; } // Locatie van de foto 

    // Foreign Key: Koppeling naar de Jager (User) die de registratie deed
    public int UserId { get; set; }
    public User? User { get; set; }
}