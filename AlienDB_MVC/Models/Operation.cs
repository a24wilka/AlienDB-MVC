namespace AlienDB_MVC.Models
{
    // Modell för operationer
    public class Operation
    {
        public int ID { get; set; }

        public string? Kodnamn { get; set; }

        public DateTime Startdatum { get; set; }

        public DateTime? Slutdatum { get; set; }

        public int LedareID { get; set; }

        public int IncidentID { get; set; }

        public bool SuccessRate { get; set; }

        // Visas i dashboard/lista
        public string? LedareNamn { get; set; }

        public string? IncidentNamn { get; set; }
    }
}