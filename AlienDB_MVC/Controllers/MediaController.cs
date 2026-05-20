using AlienDB_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace AlienDB_MVC.Controllers
{
    public class MediaController : Controller
    {
        string connectionString =
            "server=localhost;database=alien_db;user=wkabuye;password=Willis123!;";

        // ================================
        // VISA ALL MEDIA
        // ================================
        public IActionResult Index(string search)
        {
            // Kontroll om användaren är inloggad
            if (HttpContext.Session.GetString("User") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            List<Media> mediaLista = new List<Media>();

            using (MySqlConnection conn =
                   new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            SELECT *
            FROM Media
            WHERE
                @search IS NULL
                OR @search = ''
                OR Titel LIKE @searchValue
                OR Typ LIKE @searchValue
                OR Språk LIKE @searchValue
                OR Ägare LIKE @searchValue
                OR Land LIKE @searchValue
            ORDER BY Trovärdighet DESC
        ";

                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@search", search ?? "");
                cmd.Parameters.AddWithValue("@searchValue", "%" + search + "%");

                using (MySqlDataReader reader =
                       cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Media media = new Media();

                        media.ID = Convert.ToInt32(reader["ID"]);
                        media.Titel = reader["Titel"].ToString();
                        media.Typ = reader["Typ"].ToString();
                        media.Språk = reader["Språk"].ToString();
                        media.Ägare = reader["Ägare"].ToString();
                        media.Land = reader["Land"].ToString();
                        media.Trovärdighet = Convert.ToInt32(reader["Trovärdighet"]);
                        media.Aktiv = Convert.ToBoolean(reader["Aktiv"]);

                        mediaLista.Add(media);
                    }
                }
            }

            ViewBag.Search = search;

            return View(mediaLista);
        }

        // ================================
        // SKAPA MEDIA - FORMULÄR
        // ================================
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

            return View();
        }

        // ================================
        // SKAPA MEDIA - SPARA
        // ================================
        [HttpPost]
        public IActionResult Create(Media media)
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
            INSERT INTO Media
            (Titel, Typ, Språk, Ägare, Land, Trovärdighet, Aktiv)
            VALUES
            (@Titel, @Typ, @Språk, @Ägare, @Land, @Trovärdighet, @Aktiv)
        ";

                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@Titel", media.Titel);
                cmd.Parameters.AddWithValue("@Typ", media.Typ);
                cmd.Parameters.AddWithValue("@Språk", media.Språk);
                cmd.Parameters.AddWithValue("@Ägare", media.Ägare);
                cmd.Parameters.AddWithValue("@Land", media.Land);
                cmd.Parameters.AddWithValue("@Trovärdighet", media.Trovärdighet);
                cmd.Parameters.AddWithValue("@Aktiv", media.Aktiv);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
        // ================================
        // REDIGERA MEDIA - FORMULÄR
        // ================================
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

            Media media = new Media();

            using (MySqlConnection conn =
                   new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            SELECT *
            FROM Media
            WHERE ID = @ID
        ";

                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ID", id);

                using (MySqlDataReader reader =
                       cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        media.ID =
                            Convert.ToInt32(reader["ID"]);

                        media.Titel =
                            reader["Titel"].ToString();

                        media.Typ =
                            reader["Typ"].ToString();

                        media.Språk =
                            reader["Språk"].ToString();

                        media.Ägare =
                            reader["Ägare"].ToString();

                        media.Land =
                            reader["Land"].ToString();

                        media.Trovärdighet =
                            Convert.ToInt32(reader["Trovärdighet"]);

                        media.Aktiv =
                            Convert.ToBoolean(reader["Aktiv"]);
                    }
                }
            }

            return View(media);
        }

        // ================================
        // REDIGERA MEDIA - SPARA
        // ================================
        [HttpPost]
        public IActionResult Edit(Media media)
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
            UPDATE Media
            SET
                Titel = @Titel,
                Typ = @Typ,
                Språk = @Språk,
                Ägare = @Ägare,
                Land = @Land,
                Trovärdighet = @Trovärdighet,
                Aktiv = @Aktiv
            WHERE ID = @ID
        ";

                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ID", media.ID);
                cmd.Parameters.AddWithValue("@Titel", media.Titel);
                cmd.Parameters.AddWithValue("@Typ", media.Typ);
                cmd.Parameters.AddWithValue("@Språk", media.Språk);
                cmd.Parameters.AddWithValue("@Ägare", media.Ägare);
                cmd.Parameters.AddWithValue("@Land", media.Land);
                cmd.Parameters.AddWithValue("@Trovärdighet", media.Trovärdighet);
                cmd.Parameters.AddWithValue("@Aktiv", media.Aktiv);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
        // ================================
        // TAR BORT MEDIA
        // ================================
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

            try
            {
                using (MySqlConnection conn =
                       new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                DELETE FROM Media
                WHERE ID = @ID
            ";

                    MySqlCommand cmd =
                        new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@ID", id);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException)
            {
                TempData["Error"] =
                    "Media kan inte tas bort eftersom den används i nyhetsrapporter.";

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}