using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using AlienDB_MVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlienDB_MVC.Controllers
{
    // Controller för operationer
    public class OperationController : Controller
    {
        // Databasanslutning
        string connectionString =
            "server=localhost;database=alien_db;user=wkabuye;password=Willis123!;";

        // =========================================
        // LISTA ALLA OPERATIONER
        // =========================================
        public IActionResult Index(string search)
        {
            // Kontroll om användaren är inloggad
            if (HttpContext.Session.GetString("User") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Lista med operationer
            List<Operation> operationer = new();

            using (MySqlConnection conn =
                   new MySqlConnection(connectionString))
            {
                conn.Open();

                // SQL-fråga för att hämta operationer
                string query = @"

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
        o.Kodnamn LIKE @search
        OR a.Name LIKE @search
        OR i.Namn LIKE @search
";

                // SQL-kommando
                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                // Sökvärde från formuläret
                cmd.Parameters.AddWithValue(
                    "@search",
                    "%" + search + "%"
                );

                MySqlDataReader reader =
                    cmd.ExecuteReader();

                while (reader.Read())
                {
                    operationer.Add(new Operation
                    {
                        ID =
                            Convert.ToInt32(reader["ID"]),

                        Kodnamn =
                            reader["Kodnamn"].ToString(),

                        Startdatum =
                            Convert.ToDateTime(reader["Startdatum"]),

                        Slutdatum =
                            reader["Slutdatum"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(reader["Slutdatum"]),

                        SuccessRate =
                            Convert.ToBoolean(reader["SuccessRate"]),

                        LedareNamn =
                            reader["LedareNamn"] == DBNull.Value
                                ? ""
                                : reader["LedareNamn"].ToString(),

                        IncidentNamn =
                            reader["IncidentNamn"] == DBNull.Value
                                ? ""
                                : reader["IncidentNamn"].ToString()
                    });
                }
            }

            return View(operationer);
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

            // Bara Commander/Admin får skapa
            if (HttpContext.Session.GetString("Role") != "Admin" &&
                HttpContext.Session.GetString("Role") != "Commander")
            {
                return RedirectToAction("AccessDenied", "Login");
            }

            LoadDropdowns();

            return View();
        }
        // Laddar dropdown-data
        private void LoadDropdowns()
        {
            using (MySqlConnection conn =
          new MySqlConnection(connectionString))
            {
                conn.Open();

                // =========================
                // Ledare / Agenter
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
            }

        }
        // =========================================
        // CREATE - POST
        // =========================================

        // Sparar operation i databasen
        [HttpPost]
        public IActionResult Create(Operation operation)
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

            using (MySqlConnection conn =
                   new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            INSERT INTO Operationer
            (
                Kodnamn,
                Startdatum,
                Slutdatum,
                LedareID,
                IncidentID,
                SuccessRate
            )

            VALUES
            (
                @Kodnamn,
                @Startdatum,
                @Slutdatum,
                @LedareID,
                @IncidentID,
                @SuccessRate
            )
        ";

                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@Kodnamn",
                    operation.Kodnamn);

                cmd.Parameters.AddWithValue("@Startdatum",
                    operation.Startdatum);

                cmd.Parameters.AddWithValue("@Slutdatum",
                    operation.Slutdatum);

                cmd.Parameters.AddWithValue("@LedareID",
                    operation.LedareID);

                cmd.Parameters.AddWithValue("@IncidentID",
                    operation.IncidentID);

                cmd.Parameters.AddWithValue("@SuccessRate",
                    operation.SuccessRate);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
        // =========================================
        // EDIT - GET
        // =========================================

        // Visar vald operation för redigering
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

            Operation operation = new();

            using (MySqlConnection conn =
                   new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            SELECT *
            FROM Operationer
            WHERE ID = @ID
        ";

                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ID", id);

                MySqlDataReader reader =
                    cmd.ExecuteReader();

                if (reader.Read())
                {
                    operation.ID =
                        Convert.ToInt32(reader["ID"]);

                    operation.Kodnamn =
                        reader["Kodnamn"].ToString();

                    operation.Startdatum =
                        Convert.ToDateTime(reader["Startdatum"]);

                    operation.Slutdatum =
                        reader["Slutdatum"] == DBNull.Value
                            ? null
                            : Convert.ToDateTime(reader["Slutdatum"]);

                    operation.LedareID =
                        Convert.ToInt32(reader["LedareID"]);

                    operation.IncidentID =
                        Convert.ToInt32(reader["IncidentID"]);

                    operation.SuccessRate =
                        Convert.ToBoolean(reader["SuccessRate"]);
                }
            }

            LoadDropdowns();

            return View(operation);
        }
        // =========================================
        // EDIT - POST
        // =========================================

        // Sparar ändringar för operation
        [HttpPost]
        public IActionResult Edit(Operation operation)
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
            UPDATE Operationer
            SET
                Kodnamn = @Kodnamn,
                Startdatum = @Startdatum,
                Slutdatum = @Slutdatum,
                LedareID = @LedareID,
                IncidentID = @IncidentID,
                SuccessRate = @SuccessRate
            WHERE ID = @ID
        ";

                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ID", operation.ID);
                cmd.Parameters.AddWithValue("@Kodnamn", operation.Kodnamn);
                cmd.Parameters.AddWithValue("@Startdatum", operation.Startdatum);
                cmd.Parameters.AddWithValue("@Slutdatum", operation.Slutdatum);
                cmd.Parameters.AddWithValue("@LedareID", operation.LedareID);
                cmd.Parameters.AddWithValue("@IncidentID", operation.IncidentID);
                cmd.Parameters.AddWithValue("@SuccessRate", operation.SuccessRate);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
        // =========================================
        // DELETE
        // =========================================

        // Tar bort operation
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
            DELETE FROM Operationer
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