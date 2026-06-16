namespace FaunaConnect2.Api.Models;

public class HuntingGround
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double TotalAreaInHectares { get; set; } // Automatically calculated
        
    // We store the drawn boundaries (coordinates) simply as JSON text
    public string PolygonCoordinatesJson { get; set; } = string.Empty;

    // Which hunter belongs to this hunting ground?
    public int UserId { get; set; }
    public User? User { get; set; }
}