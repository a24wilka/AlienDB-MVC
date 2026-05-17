namespace AlienDB_MVC.Models
{
    // Modellklass för aliens
    public class Alien
    {
        // Primärnyckel / unikt ID för alien
        public int ID { get; set; }

        // Alienens namn
        public string? Namn { get; set; }

        // ID för rasen som alien tillhör
        public int RasID { get; set; }

        // Namn på alienens ras
        public string? RasNamn { get; set; }

        // Alienens favoritvapen
        public string? FavoritVapen { get; set; }

        // Hur många armar alienen har
        public int AntalArmar { get; set; }

        // Alienens farlighetsgrad
        public string? Farlighetsgrad { get; set; }
    }
}