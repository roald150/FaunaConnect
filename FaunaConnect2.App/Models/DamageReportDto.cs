using System;

namespace FaunaConnect2.App.Models;

public class DamageReportDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTime Timestamp { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
}