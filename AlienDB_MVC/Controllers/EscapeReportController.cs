using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using AlienDB_MVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlienDB_MVC.Controllers
{
    // Controller för escape reports
    public class EscapeReportController : Controller
    {
        // Databasanslutning
        string connectionString =
            "server=localhost;database=alien_db;user=wkabuye;password=Willis123!;";

        // =========================================
        // INDEX - GET
        // =========================================
        public IActionResult Index(string search)
        {
            // Kontroll om användaren är inloggad
            if (HttpContext.Session.GetString("User") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            List<EscapeReport> reports = new();

            using (MySqlConnection conn =
                   new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            SELECT
                u.ID,
                u.AlienID,
                a.Namn AS AlienNamn,
                u.Datum,
                u.Anledning,
                u.Resultat,
                u.Status,
                u.Plats,
                u.DangerLevel

            FROM Undanflykt u

            JOIN Aliens a
                ON u.AlienID = a.ID

            WHERE
                @search IS NULL
                OR @search = ''
                OR a.Namn LIKE @searchValue
                OR u.Anledning LIKE @searchValue
                OR u.Resultat LIKE @searchValue
                OR u.Status LIKE @searchValue
                OR u.Plats LIKE @searchValue

            ORDER BY u.Datum DESC
        ";

                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@search", search ?? "");
                cmd.Parameters.AddWithValue("@searchValue", "%" + search + "%");

                MySqlDataReader reader =
                    cmd.ExecuteReader();

                while (reader.Read())
                {
                    reports.Add(new EscapeReport
                    {
                        ID =
                            Convert.ToInt32(reader["ID"]),

                        AlienID =
                            Convert.ToInt32(reader["AlienID"]),

                        AlienNamn =
                            reader["AlienNamn"].ToString(),

                        Datum =
                            Convert.ToDateTime(reader["Datum"]),

                        Anledning =
                            reader["Anledning"].ToString(),

                        Resultat =
                            reader["Resultat"].ToString(),

                        Status =
                            reader["Status"].ToString(),

                        Plats =
                            reader["Plats"].ToString(),

                        DangerLevel =
                            Convert.ToInt32(reader["DangerLevel"])
                    });
                }
            }

            ViewBag.Search = search;

            return View(reports);
        }

        // =========================================
        // CREATE - GET
        // =========================================
        public IActionResult Create()
        {
            // Kontroll om användaren är inloggad
            // Bara Commander/Admin får skapa
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

            // Laddar dropdown med aliens
            LoadAlienDropdown();

            return View();
        }

        // =========================================
        // CREATE - POST
        // =========================================
        [HttpPost]
        public IActionResult Create(EscapeReport report)
        {
            // Kontroll om användaren är inloggad
            // Bara Commander/Admin får skapa
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

            using (MySqlConnection conn =
                   new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            INSERT INTO Undanflykt
            (
                AlienID,
                Datum,
                Anledning,
                Resultat,
                Status,
                Plats,
                DangerLevel
            )

            VALUES
            (
                @AlienID,
                @Datum,
                @Anledning,
                @Resultat,
                @Status,
                @Plats,
                @DangerLevel
            )
        ";

                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@AlienID", report.AlienID);
                cmd.Parameters.AddWithValue("@Datum", report.Datum);
                cmd.Parameters.AddWithValue("@Anledning", report.Anledning);
                cmd.Parameters.AddWithValue("@Resultat", report.Resultat);
                cmd.Parameters.AddWithValue("@Status", report.Status);
                cmd.Parameters.AddWithValue("@Plats", report.Plats);
                cmd.Parameters.AddWithValue("@DangerLevel", report.DangerLevel);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // =========================================
        // EDIT - GET
        // =========================================
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

            EscapeReport report = new();

            using (MySqlConnection conn =
                   new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            SELECT *
            FROM Undanflykt
            WHERE ID = @ID
        ";

                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ID", id);

                MySqlDataReader reader =
                    cmd.ExecuteReader();

                if (reader.Read())
                {
                    report.ID =
                        Convert.ToInt32(reader["ID"]);

                    report.AlienID =
                        Convert.ToInt32(reader["AlienID"]);

                    report.Datum =
                        Convert.ToDateTime(reader["Datum"]);

                    report.Anledning =
                        reader["Anledning"].ToString();

                    report.Resultat =
                        reader["Resultat"].ToString();

                    report.Status =
                        reader["Status"].ToString();

                    report.Plats =
                        reader["Plats"].ToString();

                    report.DangerLevel =
                        Convert.ToInt32(reader["DangerLevel"]);
                }
            }

            LoadAlienDropdown();

            return View(report);
        }
        // =========================================
        // EDIT - POST
        // =========================================
        [HttpPost]
        public IActionResult Edit(EscapeReport report)
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
            UPDATE Undanflykt
            SET
                AlienID = @AlienID,
                Datum = @Datum,
                Anledning = @Anledning,
                Resultat = @Resultat,
                Status = @Status,
                Plats = @Plats,
                DangerLevel = @DangerLevel

            WHERE ID = @ID
        ";

                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ID", report.ID);
                cmd.Parameters.AddWithValue("@AlienID", report.AlienID);
                cmd.Parameters.AddWithValue("@Datum", report.Datum);
                cmd.Parameters.AddWithValue("@Anledning", report.Anledning);
                cmd.Parameters.AddWithValue("@Resultat", report.Resultat);
                cmd.Parameters.AddWithValue("@Status", report.Status);
                cmd.Parameters.AddWithValue("@Plats", report.Plats);
                cmd.Parameters.AddWithValue("@DangerLevel", report.DangerLevel);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // =========================================
        // DELETE - GET
        // =========================================
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
            DELETE FROM Undanflykt
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
        // DROPDOWN DATA
        // =========================================
        // Laddar dropdown med aliens
        private void LoadAlienDropdown()
        {
            List<SelectListItem> aliens = new();

            using (MySqlConnection conn =
                   new MySqlConnection(connectionString))
            {
                conn.Open();

                string query =
                    "SELECT ID, Namn FROM Aliens";

                using (MySqlCommand cmd =
                       new MySqlCommand(query, conn))

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
            }

            ViewBag.Aliens = aliens;
        }
    }
}