using AlienDB_MVC.Data;
using AlienDB_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace AlienDB_MVC.Controllers
{
    // Controller för startsidan/dashboarden
    public class HomeController : Controller
    {
        // Databaskoppling
        private readonly Db _db;

        // Konstruktor som tar emot databasklassen via dependency injection
        public HomeController(Db db)
        {
            _db = db;
        }

        // Startsidan
        public IActionResult Index()
        {
            // Kontroll om användaren är inloggad
            if (HttpContext.Session.GetString("User") == null)
            {
                return RedirectToAction(
                    "Index",
                    "Login");
            }

            // Modell som innehåller statistik till dashboarden
            DashboardModel dashboard = new DashboardModel();

            // Hämtar databasanslutning
            using var conn = _db.GetConnection();

            // Öppnar anslutningen mot MySQL
            conn.Open();

            // =============================================
            // SENASTE MEDIA
            // =============================================

            string mediaQuery = @"
    SELECT *
    FROM Media
    ORDER BY ID DESC
    LIMIT 3
";

            MySqlCommand mediaCmd =
                new MySqlCommand(mediaQuery, conn);

            using (MySqlDataReader reader =
                   mediaCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    dashboard.LatestMedia.Add(new Media
                    {
                        ID = Convert.ToInt32(reader["ID"]),

                        Titel = reader["Titel"].ToString(),

                        Typ = reader["Typ"].ToString(),

                        Ägare = reader["Ägare"].ToString(),

                        Trovärdighet =
                            Convert.ToInt32(reader["Trovärdighet"])
                    });
                }
            }

            // =============================================
            // Hämtar antal agenter
            // =============================================

            string sqlAgents = "SELECT COUNT(*) FROM Agenter";

            using (var cmd = new MySqlCommand(sqlAgents, conn))
            {
                dashboard.AgentCount =
                    Convert.ToInt32(cmd.ExecuteScalar());
            }

            // =============================================
            // Hämtar antal aliens
            // =============================================

            string sqlAliens = "SELECT COUNT(*) FROM Aliens";

            using (var cmd = new MySqlCommand(sqlAliens, conn))
            {
                dashboard.AlienCount = Convert.ToInt32(cmd.ExecuteScalar());
            }

            // =============================================
            // Hämtar antal operationer
            // =============================================

            string sqlOperations = "SELECT COUNT(*) FROM Operationer";

            using (var cmd = new MySqlCommand(sqlOperations, conn))
            {
                dashboard.OperationCount = Convert.ToInt32(cmd.ExecuteScalar());
            }

            // =============================================
            // Hämtar antal incidenter
            // =============================================

            string sqlIncidents = "SELECT COUNT(*) FROM Incidenter";

            using (var cmd = new MySqlCommand(sqlIncidents, conn))
            {
                dashboard.IncidentCount = Convert.ToInt32(cmd.ExecuteScalar());
            }

            // =============================================
            // Hämtar senaste observationer
            // =============================================

            string latestObsQuery = @"
    SELECT
        o.ID AS ObservationID,
        a.Name AS AgentNamn,
        al.Namn AS AlienNamn,
        i.Namn AS IncidentNamn,
        i.Plats,
        i.Region,
        op.Kodnamn AS OperationKodnamn,
        o.Datum,
        o.Säkerhet,
        o.Grad

    FROM Observationer o

    LEFT JOIN Agenter a
        ON o.HandläggareID = a.ID

    LEFT JOIN Aliens al
        ON o.AlienID = al.ID

    LEFT JOIN Incidenter i
        ON o.IncidentID = i.ID

    LEFT JOIN Operationer op
        ON i.ID = op.IncidentID

    ORDER BY o.Datum DESC

    LIMIT 5
";

            using (var cmd = new MySqlCommand(latestObsQuery, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    dashboard.LatestObservations.Add(
                        new ObservationDashboard
                        {
                            ObservationID =
                                Convert.ToInt32(reader["ObservationID"]),

                            AgentNamn =
                                reader["AgentNamn"]?.ToString(),

                            AlienNamn =
                                reader["AlienNamn"]?.ToString(),

                            IncidentNamn =
                                reader["IncidentNamn"]?.ToString(),

                            Plats =
                                reader["Plats"]?.ToString(),

                            Region =
                                reader["Region"]?.ToString(),

                            OperationKodnamn =
                                reader["OperationKodnamn"]?.ToString(),

                            Datum =
                                Convert.ToDateTime(reader["Datum"]),

                            Säkerhet =
                                Convert.ToInt32(reader["Säkerhet"]),

                            Grad =
                                Convert.ToInt32(reader["Grad"])
                        });
                }
            }
            // =============================================
            // Hämtar antal observationer
            // =============================================

            string sqlObservations = "SELECT COUNT(*) FROM Observationer";

            using (var cmd = new MySqlCommand(sqlObservations, conn))
            {
                dashboard.ObservationCount = Convert.ToInt32(cmd.ExecuteScalar());
            }

            // =============================================
            // Escape alerts
            // =============================================

            string sqlEscapeAlerts = @"
    SELECT COUNT(*)
    FROM Undanflykt
    WHERE Status = 'Escaped'
";

            using (var cmd = new MySqlCommand(sqlEscapeAlerts, conn))
            {
                dashboard.EscapeAlertCount =
                    Convert.ToInt32(cmd.ExecuteScalar());
            }

            // =============================================
            // Hämtar aktiva operationer
            // =============================================

            string activeOperationsQuery = @"
    SELECT
        o.ID,
        o.Kodnamn,
        o.Startdatum,
        o.Slutdatum,
        o.SuccessRate,

        a.Name AS LedareNamn,

        i.Namn AS IncidentNamn

    FROM Operationer o

    LEFT JOIN Agenter a
        ON o.LedareID = a.ID

    LEFT JOIN Incidenter i
        ON o.IncidentID = i.ID

    WHERE
        o.Slutdatum IS NULL
        OR o.Slutdatum >= CURDATE()

    ORDER BY o.Startdatum DESC

    LIMIT 5
";

            
            using (var cmd = new MySqlCommand(activeOperationsQuery, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    dashboard.ActiveOperations.Add(
                        new Operation
                        {
                            ID =
                                Convert.ToInt32(reader["ID"]),

                            Kodnamn =
                                reader["Kodnamn"]?.ToString(),

                            Startdatum =
                                Convert.ToDateTime(reader["Startdatum"]),

                            Slutdatum =
                                reader["Slutdatum"] == DBNull.Value
                                    ? null
                                    : Convert.ToDateTime(reader["Slutdatum"]),

                            SuccessRate =
                                Convert.ToBoolean(reader["SuccessRate"]),

                            LedareNamn =
                                reader["LedareNamn"]?.ToString(),

                            IncidentNamn =
                                reader["IncidentNamn"]?.ToString()
                        });
                }
            }

            // =============================================
            // Hämtar senaste aktivitet/loggar
            // =============================================

            string logQuery = @"
    SELECT
        Händelse,
        TabellNamn,
        Tid

    FROM Logg

    ORDER BY Tid DESC

    LIMIT 6
";

            using (var cmd = new MySqlCommand(logQuery, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    dashboard.LatestLogs.Add(
                        new ActivityLog
                        {
                            Händelse =
                                reader["Händelse"]?.ToString(),

                            TabellNamn =
                                reader["TabellNamn"]?.ToString(),

                            Tid =
                                Convert.ToDateTime(reader["Tid"])
                        });
                }
            }
            
            // Skickar dashboard-data till vyn 
            return View(dashboard);
        }
    }
}