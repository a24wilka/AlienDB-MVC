namespace AlienDB_MVC.Models
{
    // Modellklass för alien-raser
    public class Ras
    {
        // Primärnyckel / unikt ID för rasen
        public int ID { get; set; }

        // Rasens namn
        public string? Namn { get; set; }
    }
}