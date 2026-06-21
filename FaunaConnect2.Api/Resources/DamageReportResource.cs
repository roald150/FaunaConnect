namespace FaunaConnect2.Api.Resources;

public class DamageReportResource
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTime Timestamp { get; set; }
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public int? HuntingGroundId { get; set; }
}
