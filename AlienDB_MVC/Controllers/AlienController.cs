using AlienDB_MVC.Data;
using AlienDB_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlienDB_MVC.Controllers
{
    // Controller för aliensidorna
    public class AlienController : Controller
    {
        // Databaskoppling
        private readonly Db _db;

        // Konstruktor med dependency injection
        public AlienController(Db db)
        {
            _db = db;
        }

        // Visar lista över aliens + sökfunktion
        public IActionResult Index(string search)
        {
            List<Alien> aliens = new List<Alien>();

            using var conn = _db.GetConnection();
            conn.Open();

            string sql = @"
        SELECT 
            a.ID,
            a.Namn,
            a.RasID,
            r.Namn AS RasNamn,
            a.FavoritVapen,
            a.AntalArmar,
            a.Farlighetsgrad
        FROM Aliens a
        JOIN Ras r ON a.RasID = r.ID
        WHERE a.Namn LIKE @Search
           OR r.Namn LIKE @Search
           OR a.FavoritVapen LIKE @Search
           OR a.Farlighetsgrad LIKE @Search
    ";

            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@Search", "%" + search + "%");

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                aliens.Add(new Alien
                {
                    ID = reader.GetInt32("ID"),

                    Namn =
          reader["Namn"].ToString(),

                    RasID =
          Convert.ToInt32(reader["RasID"]),

                    RasNamn =
          reader["RasNamn"] == DBNull.Value
              ? ""
              : reader["RasNamn"].ToString(),

                    FavoritVapen =
          reader["FavoritVapen"] == DBNull.Value
              ? ""
              : reader["FavoritVapen"].ToString(),

                    AntalArmar =
          Convert.ToInt32(reader["AntalArmar"]),

                    Farlighetsgrad =
          reader["Farlighetsgrad"] == DBNull.Value
              ? ""
              : reader["Farlighetsgrad"].ToString()
                });
            }

            ViewBag.Search = search;

            return View(aliens);
        }

       
        // Visar formulär för att skapa ny alien
        public IActionResult Create()
        {
            // Lista för alla raser
            List<Ras> raser = new List<Ras>();

            using var conn = _db.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM Ras";

            using var cmd = new MySqlCommand(sql, conn);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                raser.Add(new Ras
                {
                    ID = reader.GetInt32("ID"),
                    Namn = reader.GetString("Namn")
                });
            }

            // Skickar dropdown-listan till vyn
            ViewBag.Raser = new SelectList(raser, "ID", "Namn");

            return View();
        }

        // Sparar ny alien i databasen
        [HttpPost]
        public IActionResult Create(Alien alien)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string sql = @"
        INSERT INTO Aliens
        (Namn, RasID, FavoritVapen, AntalArmar, Farlighetsgrad)
        VALUES
        (@Namn, @RasID, @FavoritVapen, @AntalArmar, @Farlighetsgrad)
    ";

            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@Namn", alien.Namn);
            cmd.Parameters.AddWithValue("@RasID", alien.RasID);
            cmd.Parameters.AddWithValue("@FavoritVapen", alien.FavoritVapen);
            cmd.Parameters.AddWithValue("@AntalArmar", alien.AntalArmar);
            cmd.Parameters.AddWithValue("@Farlighetsgrad", alien.Farlighetsgrad);

            cmd.ExecuteNonQuery();

            return RedirectToAction("Index");
        }

        // Visar formulär för att redigera alien
        public IActionResult Edit(int id)
        {
            Alien alien = new Alien();

            using var conn = _db.GetConnection();
            conn.Open();

            // Hämtar alien-data
            string sql = "SELECT * FROM Aliens WHERE ID = @ID";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ID", id);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                alien.ID =
                    Convert.ToInt32(reader["ID"]);

                alien.Namn =
                    reader["Namn"].ToString();

                alien.RasID =
                    Convert.ToInt32(reader["RasID"]);

                alien.FavoritVapen =
                    reader["FavoritVapen"] == DBNull.Value
                        ? ""
                        : reader["FavoritVapen"].ToString();

                alien.AntalArmar =
                    Convert.ToInt32(reader["AntalArmar"]);

                alien.Farlighetsgrad =
                    reader["Farlighetsgrad"] == DBNull.Value
                        ? ""
                        : reader["Farlighetsgrad"].ToString();
            }

            // Stänger reader innan nästa SQL-fråga
            reader.Close();

            // Lista för dropdown med raser
            List<Ras> raser = new List<Ras>();

            // Hämtar alla raser
            string rasSql = "SELECT * FROM Ras";

            using var rasCmd = new MySqlCommand(rasSql, conn);

            using var rasReader = rasCmd.ExecuteReader();

            while (rasReader.Read())
            {
                raser.Add(new Ras
                {
                    ID = rasReader.GetInt32("ID"),
                    Namn = rasReader.GetString("Namn")
                });
            }

            // Skickar dropdown-data till vyn
            ViewBag.Raser = new SelectList(raser, "ID", "Namn");

            return View(alien);
        }

        // Sparar ändringar för alien
        [HttpPost]
        public IActionResult Edit(Alien alien)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string sql = @"
                UPDATE Aliens
                SET
                    Namn = @Namn,
                    RasID = @RasID,
                    FavoritVapen = @FavoritVapen,
                    AntalArmar = @AntalArmar,
                    Farlighetsgrad = @Farlighetsgrad
                WHERE ID = @ID
            ";

            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@ID", alien.ID);
            cmd.Parameters.AddWithValue("@Namn", alien.Namn);
            cmd.Parameters.AddWithValue("@RasID", alien.RasID);
            cmd.Parameters.AddWithValue("@FavoritVapen", alien.FavoritVapen);
            cmd.Parameters.AddWithValue("@AntalArmar", alien.AntalArmar);
            cmd.Parameters.AddWithValue("@Farlighetsgrad", alien.Farlighetsgrad);

            cmd.ExecuteNonQuery();

            return RedirectToAction("Index");
        }

        // Visar bekräftelsesida för att ta bort alien
        public IActionResult Delete(int id)
        {
            Alien alien = new Alien();

            using var conn = _db.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM Aliens WHERE ID = @ID";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ID", id);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                alien.ID =
                    Convert.ToInt32(reader["ID"]);

                alien.Namn =
                    reader["Namn"].ToString();

                alien.RasID =
                    Convert.ToInt32(reader["RasID"]);

                alien.FavoritVapen =
                    reader["FavoritVapen"] == DBNull.Value
                        ? ""
                        : reader["FavoritVapen"].ToString();

                alien.AntalArmar =
                    Convert.ToInt32(reader["AntalArmar"]);

                alien.Farlighetsgrad =
                    reader["Farlighetsgrad"] == DBNull.Value
                        ? ""
                        : reader["Farlighetsgrad"].ToString();
            }

            return View(alien);
        }

        // Tar bort alien från databasen
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            // Tar först bort kopplade undanflykter
            string sqlUndanflykt = "DELETE FROM Undanflykt WHERE AlienID = @ID";

            using (var cmd = new MySqlCommand(sqlUndanflykt, conn))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.ExecuteNonQuery();
            }

            // Tar sedan bort alien
            string sqlAlien = "DELETE FROM Aliens WHERE ID = @ID";

            using (var cmd = new MySqlCommand(sqlAlien, conn))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}