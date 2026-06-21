namespace FaunaConnect2.Api.Resources;

public class RegistrationResource
{
    public int Id { get; set; }
    public string AnimalName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime DateTime { get; set; }
    public string? PhotoUrl { get; set; }
    public int UserId { get; set; }
    public string? UserName { get; set; }
}
