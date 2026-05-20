using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using AlienDB_MVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlienDB_MVC.Controllers
{
    // Controller för observation-dashboard
    public class ObservationController : Controller
    {
        // Anslutning till MySQL-databasen
        string connectionString =
      "server=localhost;database=alien_db;user=wkabuye;password=Willis123!;";
        
        // =========================================
        // INDEX - GET
        // =========================================
        // Hämtar dashboard-data från databasen
        public IActionResult Index(string search)
        {
            // Kontroll om användaren är inloggad
            if (HttpContext.Session.GetString("User") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Lista som ska innehålla observationerna
            List<ObservationDashboard> dashboard = new();

            // Skapar anslutning till databasen
            using (MySqlConnection conn =
                   new MySqlConnection(connectionString))
            {
                // Öppnar anslutningen
                conn.Open();

                // Hämtar data från dashboard-vyn
                string query = @"
    SELECT *
    FROM Vy_AgentAlienDashboard

    WHERE
        AgentNamn LIKE @search
        OR AlienNamn LIKE @search
        OR IncidentNamn LIKE @search
        OR OperationKodnamn LIKE @search
";

                // SQL-kommando
                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                // Sökvärde från formuläret
                cmd.Parameters.AddWithValue(
                    "@search",
                    "%" + search + "%"
                );

                // Läser resultatet från databasen
                MySqlDataReader reader =
                    cmd.ExecuteReader();

                // Loopar igenom alla rader
                while (reader.Read())
                {
                    // Lägger till observation i listan
                    dashboard.Add(new ObservationDashboard
                    {
                        ObservationID =
                            Convert.ToInt32(reader["ObservationID"]),

                        AgentNamn =
                            reader["AgentNamn"].ToString(),

                        AlienNamn =
                            reader["AlienNamn"].ToString(),

                        IncidentNamn =
                            reader["IncidentNamn"].ToString(),

                        Plats =
                            reader["Plats"].ToString(),

                        Region =
                            reader["Region"].ToString(),

                        OperationKodnamn =
                            reader["OperationKodnamn"].ToString(),

                        Datum =
                            Convert.ToDateTime(reader["Datum"]),

                        Säkerhet =
                            Convert.ToInt32(reader["Säkerhet"]),

                        Grad =
                            Convert.ToInt32(reader["Grad"])
                    });
                }
            }

            // Skickar listan till vyn
            return View(dashboard);
        }

        // =========================================
        // CREATE - GET
        // =========================================
        // Visar formuläret för att skapa en ny observation
        public IActionResult Create()
        {
            // Kontroll om användaren är inloggad
            if (HttpContext.Session.GetString("User") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Bara Commander/Admin får skapa
            if (HttpContext.Session.GetString("Role") != "Admin" &&
                HttpContext.Session.GetString("Role") != "Commander")
            {
                return RedirectToAction("AccessDenied", "Login");
            }

            LoadDropdowns();
            return View();
        }
        // =========================================
        // CREATE - POST
        // =========================================
        [HttpPost]
        public IActionResult Create(Observation observation)
        {
            // Kontroll om användaren är inloggad
            if (HttpContext.Session.GetString("User") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            // Bara Commander/Admin får skapa
            if (HttpContext.Session.GetString("Role") != "Admin" &&
                HttpContext.Session.GetString("Role") != "Commander")
            {
                return RedirectToAction("AccessDenied", "Login");
            }
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    INSERT INTO Observationer
                    (Datum, Säkerhet, Grad, HandläggareID, IncidentID, AlienID)
                    VALUES
                    (@Datum, @Säkerhet, @Grad, @HandläggareID, @IncidentID, @AlienID)
                ";

                MySqlCommand cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@Datum", observation.Datum);
                cmd.Parameters.AddWithValue("@Säkerhet", observation.Säkerhet);
                cmd.Parameters.AddWithValue("@Grad", observation.Grad);
                cmd.Parameters.AddWithValue("@HandläggareID", observation.HandläggareID);
                cmd.Parameters.AddWithValue("@IncidentID", observation.IncidentID);
                cmd.Parameters.AddWithValue("@AlienID", observation.AlienID);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }


        // =========================================
        // EDIT - GET
        // =========================================
        // Visar vald observation för redigering
        public IActionResult Edit(int id)
        {
            // Kontroll om användaren är inloggad
            if (HttpContext.Session.GetString("User") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Bara Commander/Admin får redigera
            if (HttpContext.Session.GetString("Role") != "Admin" &&
                HttpContext.Session.GetString("Role") != "Commander")
            {
                return RedirectToAction("AccessDenied", "Login");
            }
            Observation observation = new();

            using (MySqlConnection conn =
                   new MySqlConnection(connectionString))
            {
                conn.Open();

                // Hämtar observationen
                string query = @"
            SELECT *
            FROM Observationer
            WHERE ID = @ID
        ";

                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ID", id);

                MySqlDataReader reader =
                    cmd.ExecuteReader();

                if (reader.Read())
                {
                    observation.ID =
                        Convert.ToInt32(reader["ID"]);

                    observation.Datum =
                        Convert.ToDateTime(reader["Datum"]);

                    observation.Säkerhet =
                        Convert.ToInt32(reader["Säkerhet"]);

                    observation.Grad =
                        Convert.ToInt32(reader["Grad"]);

                    observation.HandläggareID =
                        Convert.ToInt32(reader["HandläggareID"]);

                    observation.IncidentID =
                        Convert.ToInt32(reader["IncidentID"]);

                    observation.AlienID =
                        Convert.ToInt32(reader["AlienID"]);
                }
            }

            // Laddar dropdown-listor för edit-formuläret
            LoadDropdowns();

            return View(observation);
        }

        // =========================================
        // EDIT - POST
        // =========================================
        // Sparar ändringar för observation
        [HttpPost]
        public IActionResult Edit(Observation observation)
        {
            // Kontroll om användaren är inloggad
            if (HttpContext.Session.GetString("User") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Bara Commander/Admin får redigera
            if (HttpContext.Session.GetString("Role") != "Admin" &&
                HttpContext.Session.GetString("Role") != "Commander")
            {
                return RedirectToAction("AccessDenied", "Login");
            }

            using (MySqlConnection conn =
                   new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            UPDATE Observationer
            SET
                Datum = @Datum,
                Säkerhet = @Säkerhet,
                Grad = @Grad,
                HandläggareID = @HandläggareID,
                IncidentID = @IncidentID,
                AlienID = @AlienID
            WHERE ID = @ID
        ";

                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ID", observation.ID);
                cmd.Parameters.AddWithValue("@Datum", observation.Datum);
                cmd.Parameters.AddWithValue("@Säkerhet", observation.Säkerhet);
                cmd.Parameters.AddWithValue("@Grad", observation.Grad);
                cmd.Parameters.AddWithValue("@HandläggareID", observation.HandläggareID);
                cmd.Parameters.AddWithValue("@IncidentID", observation.IncidentID);
                cmd.Parameters.AddWithValue("@AlienID", observation.AlienID);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // =========================================
        // DELETE - GET
        // =========================================
        // Tar bort observation
        public IActionResult Delete(int id)
        {
            // Kontroll om användaren är inloggad
            if (HttpContext.Session.GetString("User") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Bara Admin får ta bort
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("AccessDenied", "Login");
            }

            using (MySqlConnection conn =
                   new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
    DELETE FROM Observationer
    WHERE ID = @ID
";

                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ID", id);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // =========================================
        // DROPDOWN
        // =========================================

        // Laddar dropdown-data
        private void LoadDropdowns()
        {
            using (MySqlConnection conn =
                   new MySqlConnection(connectionString))
            {
                conn.Open();

                // =========================
                // Agenter
                // =========================
                List<SelectListItem> agenter = new();

                string agentQuery =
                    "SELECT ID, Name FROM Agenter";

                using (MySqlCommand cmd =
                       new MySqlCommand(agentQuery, conn))

                using (MySqlDataReader reader =
                       cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        agenter.Add(new SelectListItem
                        {
                            Value = reader["ID"].ToString(),
                            Text = reader["Name"].ToString()
                        });
                    }
                }

                ViewBag.Agenter = agenter;

                // =========================
                // Incidenter
                // =========================
                List<SelectListItem> incidenter = new();

                string incidentQuery =
                    "SELECT ID, Namn FROM Incidenter";

                using (MySqlCommand cmd =
                       new MySqlCommand(incidentQuery, conn))

                using (MySqlDataReader reader =
                       cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        incidenter.Add(new SelectListItem
                        {
                            Value = reader["ID"].ToString(),
                            Text = reader["Namn"].ToString()
                        });
                    }
                }

                ViewBag.Incidenter = incidenter;

                // =========================
                // Aliens
                // =========================
                List<SelectListItem> aliens = new();

                string alienQuery =
                    "SELECT ID, Namn FROM Aliens";

                using (MySqlCommand cmd =
                       new MySqlCommand(alienQuery, conn))

                using (MySqlDataReader reader =
                       cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        aliens.Add(new SelectListItem
                        {
                            Value = reader["ID"].ToString(),
                            Text = reader["Namn"].ToString()
                        });
                    }
                }

                ViewBag.Aliens = aliens;
            }
        }
    }
}