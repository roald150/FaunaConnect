namespace FaunaConnect2.Api.Models;

public class Registration
{
    public int Id { get; set; }
    public string AnimalName { get; set; } = string.Empty; // e.g. "Roe Deer" 
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime DateTime { get; set; } = DateTime.Now;
    public string? PhotoUrl { get; set; } // Photo location 

    // Foreign Key: Link to the Hunter (User) who made the registration
    public int UserId { get; set; }
    public User? User { get; set; }
}