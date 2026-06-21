namespace FaunaConnect2.Api.Resources;

public class HuntingGroundResource
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double TotalAreaInHectares { get; set; }
    public string PolygonCoordinatesJson { get; set; } = string.Empty;
    public int UserId { get; set; }
}
