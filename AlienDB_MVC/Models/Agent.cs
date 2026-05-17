namespace AlienDB_MVC.Models
{
    // Modell för agenter
    public class Agent
    {
        public int ID { get; set; }

        public string? Name { get; set; }

        public int Nummer { get; set; }

        public string? Specialitet { get; set; }

        public string? Roll { get; set; }

        public string? RegionKod { get; set; }

        public decimal Lon { get; set; }
    }
}