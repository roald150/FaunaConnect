using SQLite;

namespace FaunaConnect2.App.Models
{
    public class Registration
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string AnimalName { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        public string? PhotoUrl { get; set; }
        public int UserId { get; set; }
        

        public bool IsSynced { get; set; }
    }
}