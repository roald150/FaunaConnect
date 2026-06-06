namespace FaunaConnect2.App.Models
{
    public class Registration
    {
        public int Id { get; set; }
        public string AnimalName { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime DateTime { get; set; }
    }
}