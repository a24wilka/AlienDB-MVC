namespace AlienDB_MVC.Models
{
    // Modell för incidenter
    public class Incident
    {
        // Primärnyckel
        public int ID { get; set; }

        // Namn på incidenten
        public string? Namn { get; set; }

        // Plats där incidenten inträffade
        public string? Plats { get; set; }

        // Region för incidenten
        public string? Region { get; set; }
    }
}