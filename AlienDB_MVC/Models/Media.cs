namespace AlienDB_MVC.Models
{
    // Modell för media/nyhetskanaler i AlienDB
    public class Media
    {
        public int ID { get; set; }

        public string? Titel { get; set; }

        public string? Typ { get; set; }

        public string? Språk { get; set; }

        public string? Ägare { get; set; }

        public string? Land { get; set; }

        public int Trovärdighet { get; set; }

        public bool Aktiv { get; set; }
    }
}