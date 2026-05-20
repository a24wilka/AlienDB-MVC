namespace AlienDB_MVC.Models
{
    // Modell för alien-undanflykter / escape reports
    public class EscapeReport
    {
        public int ID { get; set; }

        public int AlienID { get; set; }

        public string? AlienNamn { get; set; }

        public DateTime Datum { get; set; }

        public string? Anledning { get; set; }

        public string? Resultat { get; set; }

        public string? Status { get; set; }

        public string? Plats { get; set; }

        public int DangerLevel { get; set; }
    }
}