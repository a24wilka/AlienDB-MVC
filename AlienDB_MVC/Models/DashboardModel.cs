namespace AlienDB_MVC.Models
{
    // Modell för dashboard/statistik på startsidan
    public class DashboardModel
    {
        // Totalt antal agenter i databasen
        public int AgentCount { get; set; }

        // Totalt antal aliens i databasen
        public int AlienCount { get; set; }

        // Totalt antal operationer
        public int OperationCount { get; set; }

        // Totalt antal incidenter
        public int IncidentCount { get; set; }

        // Totalt antal observationer
        public int ObservationCount { get; set; }

        // Senaste observationer till dashboarden
        public List<ObservationDashboard> LatestObservations { get; set; } = new();

        // Aktiva operationer till dashboarden
        public List<Operation> ActiveOperations { get; set; } = new();

        // Senaste loggar / aktivitet
        public List<ActivityLog> LatestLogs { get; set; } = new();
        
    }
}