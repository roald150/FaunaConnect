namespace FaunaConnect2.Api.Models;

public class HuntingGround
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double TotalAreaInHectares { get; set; } // Automatisch berekend [cite: 19, 33]
        
    // We slaan de getekende grenzen (coördinaten) simpelweg op als JSON tekst
    public string PolygonCoordinatesJson { get; set; } = string.Empty;

    // Welke jager hoort bij dit jachtgebied?
    public int UserId { get; set; }
    public User? User { get; set; }
}