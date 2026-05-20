using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using AlienDB_MVC.Models;

namespace AlienDB_MVC.Controllers
{
    // Controller för incidenter
    public class IncidentController : Controller
    {
        // Anslutning till databasen
        string connectionString =
            "server=localhost;database=alien_db;user=wkabuye;password=Willis123!;";

        // =========================================
        // LISTA ALLA INCIDENTER
        // =========================================
        public IActionResult Index(string search)
        {
            // Kontroll om användaren är inloggad
            if (HttpContext.Session.GetString("User") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Lista med incidenter
            List<Incident> incidenter = new();

            using (MySqlConnection conn =
                   new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
    SELECT *
    FROM Incidenter
    WHERE
        Namn LIKE @search
        OR Plats LIKE @search
        OR Region LIKE @search
";

                MySqlCommand cmd =
    new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue(
                    "@search",
                    "%" + search + "%"
                );

                MySqlDataReader reader =
                    cmd.ExecuteReader();

                // Loopar igenom alla incidenter
                while (reader.Read())
                {
                    incidenter.Add(new Incident
                    {
                        ID =
                            Convert.ToInt32(reader["ID"]),

                        Namn =
                            reader["Namn"].ToString(),

                        Plats =
                            reader["Plats"].ToString(),

                        Region =
                            reader["Region"].ToString()
                    });
                }
            }

            return View(incidenter);
        }
        // =========================================
        // CREATE - GET
        // =========================================

        // Visar formuläret
        public IActionResult Create()
        {
            // Kontroll om användaren är inloggad
            if (HttpContext.Session.GetString("User") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Bara Commander/Admin får skapa/redigera
            if (HttpContext.Session.GetString("Role") != "Admin" &&
                HttpContext.Session.GetString("Role") != "Commander")
            {
                return RedirectToAction("AccessDenied", "Login");
            }

            return View();
        }

        // =========================================
        // CREATE - POST
        // =========================================

        // Sparar incident i databasen
        [HttpPost]
        public IActionResult Create(Incident incident)
        {
            // Kontroll om användaren är inloggad
            if (HttpContext.Session.GetString("User") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Bara Commander/Admin får skapa/redigera
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
            INSERT INTO Incidenter
            (Namn, Plats, Region)

            VALUES
            (@Namn, @Plats, @Region)
        ";

                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@Namn",
                    incident.Namn);

                cmd.Parameters.AddWithValue("@Plats",
                    incident.Plats);

                cmd.Parameters.AddWithValue("@Region",
                    incident.Region);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // =========================================
        // EDIT - GET
        // =========================================

        // Visar vald incident för redigering
        public IActionResult Edit(int id)
        {
            // Kontroll om användaren är inloggad
            if (HttpContext.Session.GetString("User") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Bara Commander/Admin får skapa/redigera
            if (HttpContext.Session.GetString("Role") != "Admin" &&
                HttpContext.Session.GetString("Role") != "Commander")
            {
                return RedirectToAction("AccessDenied", "Login");
            }

            Incident incident = new();

            using (MySqlConnection conn =
                   new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            SELECT *
            FROM Incidenter
            WHERE ID = @ID
        ";

                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ID", id);

                MySqlDataReader reader =
                    cmd.ExecuteReader();

                if (reader.Read())
                {
                    incident.ID =
                        Convert.ToInt32(reader["ID"]);

                    incident.Namn =
                        reader["Namn"].ToString();

                    incident.Plats =
                        reader["Plats"].ToString();

                    incident.Region =
                        reader["Region"].ToString();
                }
            }

            return View(incident);
        }

        // =========================================
        // EDIT - POST
        // =========================================

        // Sparar ändringar
        [HttpPost]
        public IActionResult Edit(Incident incident)
        {
            // Kontroll om användaren är inloggad
            if (HttpContext.Session.GetString("User") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Bara Commander/Admin får skapa/redigera
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
            UPDATE Incidenter
            SET
                Namn = @Namn,
                Plats = @Plats,
                Region = @Region
            WHERE ID = @ID
        ";

                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ID",
                    incident.ID);

                cmd.Parameters.AddWithValue("@Namn",
                    incident.Namn);

                cmd.Parameters.AddWithValue("@Plats",
                    incident.Plats);

                cmd.Parameters.AddWithValue("@Region",
                    incident.Region);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // =========================================
        // DELETE
        // =========================================

        // Tar bort incident
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
            DELETE FROM Incidenter
            WHERE ID = @ID
        ";

                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ID", id);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}