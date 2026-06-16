namespace FaunaConnect2.Api.Models;

public class AnimalSpecies
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
}