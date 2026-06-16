using System;

namespace FaunaConnect2.Api.Models;

public class DamageReport
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;

    // Wie heeft het gemeld (Boer)
    public int UserId { get; set; }
    public User? User { get; set; }

    // Bij welk jachtgebied hoort het (optioneel)
    public int? HuntingGroundId { get; set; }
    public HuntingGround? HuntingGround { get; set; }
}