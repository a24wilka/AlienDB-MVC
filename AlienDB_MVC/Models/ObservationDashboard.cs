namespace AlienDB_MVC.Models
{
    // Modell för dashboard som visar observationer
    public class ObservationDashboard
    {
        public int ObservationID { get; set; }

        public string? AgentNamn { get; set; }

        public string? AlienNamn { get; set; }

        public string? IncidentNamn { get; set; }

        public string? Plats { get; set; }

        public string? Region { get; set; }

        public string? OperationKodnamn { get; set; }

        public DateTime Datum { get; set; }

        public int Säkerhet { get; set; }

        public int Grad { get; set; }
    }
}