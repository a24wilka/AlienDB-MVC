using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;

namespace AlienDB_MVC.Controllers
{
    public class LoginController : Controller
    {
        // Databasanslutning
        string connectionString =
            "server=localhost;database=alien_db;user=wkabuye;password=Willis123!;";
        // ================================
        // LOGIN PAGE
        // ================================
        public IActionResult Index()
        {
            return View();
        }

        // ================================
        // LOGIN POST
        // ================================
        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            // Skapar anslutning till databasen
            using (MySqlConnection conn =
                   new MySqlConnection(connectionString))
            {
                // Öppnar anslutningen
                conn.Open();

                // Hämtar användare från Users-tabellen
                string query = @"
            SELECT Username, Role
            FROM Users
            WHERE Username = @Username
              AND Password = @Password
        ";

                // SQL-kommando
                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                // SQL-parametrar
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                // Läser resultatet från databasen
                using (MySqlDataReader reader =
                       cmd.ExecuteReader())
                {
                    // Kontroll om användaren finns
                    if (reader.Read())
                    {
                        // Sparar användarnamn i session
                        HttpContext.Session.SetString(
                            "User",
                            reader["Username"].ToString() ?? "");

                        // Sparar användarroll i session
                        HttpContext.Session.SetString(
                            "Role",
                            reader["Role"].ToString() ?? "");

                        // Redirect till dashboard
                        return RedirectToAction(
                            "Index",
                            "Home");
                    }
                }
            }

            // Felmeddelande vid fel login
            ViewBag.Error =
                "Fel användarnamn eller lösenord.";

            return View();
        }

        // ================================
        // LOGOUT
        // ================================
        public IActionResult Logout()
        {
            // Tar bort session
            HttpContext.Session.Clear();

            // Tillbaka till login
            return RedirectToAction(
                "Index",
                "Login");
        }

        // ================================
        // ACCESS DENIED
        // ================================
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}