using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using AlienDB_MVC.Models;

namespace AlienDB_MVC.Controllers
{
    // Controller för agenter
    public class AgentController : Controller
    {
        // Databasanslutning
        string connectionString =
            "server=localhost;database=alien_db;user=wkabuye;password=Willis123!;";

        // =========================================
        // LISTA ALLA AGENTER
        // =========================================
        public IActionResult Index(string search)
        {
            // Lista med agenter
            List<Agent> agenter = new();

            using (MySqlConnection conn =
                   new MySqlConnection(connectionString))
            {
                conn.Open();

                // Hämtar agenter + lön från AgentLon
                string query = @"
    SELECT 
        a.ID,
        a.Name,
        a.Nummer,
        a.Specialitet,
        a.Roll,
        a.RegionKod,
        l.Lon
    FROM Agenter a
    LEFT JOIN AgentLon l
        ON a.ID = l.AgentID
    WHERE
        @Search IS NULL
        OR @Search = ''
        OR a.Name LIKE @SearchValue
        OR a.Specialitet LIKE @SearchValue
        OR a.Roll LIKE @SearchValue
        OR a.RegionKod LIKE @SearchValue
        OR CAST(a.Nummer AS CHAR) LIKE @SearchValue
";

                MySqlCommand cmd =
                    new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Search", search ?? "");
                cmd.Parameters.AddWithValue("@SearchValue", "%" + search + "%");
                MySqlDataReader reader =
                    cmd.ExecuteReader();

                // Loopar igenom alla agenter
                while (reader.Read())
                {
                    agenter.Add(new Agent
                    {
                        ID =
           Convert.ToInt32(reader["ID"]),

                        Name =
           reader["Name"].ToString(),

                        Nummer =
           Convert.ToInt32(reader["Nummer"]),

                        Specialitet =
           reader["Specialitet"].ToString(),

                        Roll =
           reader["Roll"].ToString(),

                        RegionKod =
           reader["RegionKod"].ToString(),

                        Lon =
           reader["Lon"] == DBNull.Value
               ? 0
               : Convert.ToDecimal(reader["Lon"])
                    });
                }
            }

            return View(agenter);
        }
        // =========================================
        // CREATE - GET
        // =========================================
        // Visar formuläret för att skapa agent
        public IActionResult Create()
        {
            return View();
        }

        // =========================================
        // CREATE - POST
        // =========================================
        // Sparar ny agent i databasen
        [HttpPost]
        public IActionResult Create(Agent agent)
        {
            try
            {
                using (MySqlConnection conn =
                       new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string agentSql = @"
                INSERT INTO Agenter
                (Name, Nummer, Specialitet, Roll, RegionKod)

                VALUES
                (@Name, @Nummer, @Specialitet, @Roll, @RegionKod)
            ";

                    MySqlCommand cmd =
                        new MySqlCommand(agentSql, conn);

                    cmd.Parameters.AddWithValue("@Name", agent.Name);

                    cmd.Parameters.AddWithValue("@Nummer", agent.Nummer);

                    cmd.Parameters.AddWithValue("@Specialitet", agent.Specialitet);

                    cmd.Parameters.AddWithValue("@Roll", agent.Roll);

                    cmd.Parameters.AddWithValue("@RegionKod", agent.RegionKod);

                    cmd.ExecuteNonQuery();

                    // Hämtar ID för senaste agenten
                    long newAgentId = cmd.LastInsertedId;

                    // Sparar lön i AgentLon-tabellen
                    string lonSql = @"
                INSERT INTO AgentLon
                (AgentID, Lon)

                VALUES
                (@AgentID, @Lon)
            ";

                    MySqlCommand lonCmd =
                        new MySqlCommand(lonSql, conn);

                    lonCmd.Parameters.AddWithValue("@AgentID", newAgentId);

                    lonCmd.Parameters.AddWithValue("@Lon", agent.Lon);

                    lonCmd.ExecuteNonQuery();
                }

                return RedirectToAction("Index");
            }

            catch (MySqlException)
            {
                // Felmeddelande om agentnummer redan finns
                ViewBag.Error =
                    "Agentnummer finns redan.";

                return View(agent);
            }
        }
        // =========================================
        // EDIT - GET
        // =========================================

        // Visar vald agent för redigering
        public IActionResult Edit(int id)
        {
            Agent agent = new();

            using (MySqlConnection conn =
                   new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            SELECT
                a.ID,
                a.Name,
                a.Nummer,
                a.Specialitet,
                a.Roll,
                a.RegionKod,
                l.Lon

            FROM Agenter a

            LEFT JOIN AgentLon l
                ON a.ID = l.AgentID

            WHERE a.ID = @ID
        ";

                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ID", id);

                MySqlDataReader reader =
                    cmd.ExecuteReader();

                if (reader.Read())
                {
                    agent.ID =
                        Convert.ToInt32(reader["ID"]);

                    agent.Name =
                        reader["Name"].ToString();

                    agent.Nummer =
                        Convert.ToInt32(reader["Nummer"]);

                    agent.Specialitet =
                        reader["Specialitet"].ToString();

                    agent.Roll =
                        reader["Roll"].ToString();

                    agent.RegionKod =
                        reader["RegionKod"].ToString();

                    agent.Lon =
                        reader["Lon"] == DBNull.Value
                            ? 0
                            : Convert.ToDecimal(reader["Lon"]);
                }
            }

            return View(agent);
        }

        // =========================================
        // EDIT - POST
        // =========================================

        // Sparar ändringar för agent
        [HttpPost]
        public IActionResult Edit(Agent agent)
        {
            try
            {
                using (MySqlConnection conn =
                       new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string agentSql = @"
                UPDATE Agenter
                SET
                    Name = @Name,
                    Nummer = @Nummer,
                    Specialitet = @Specialitet,
                    Roll = @Roll,
                    RegionKod = @RegionKod
                WHERE ID = @ID
            ";

                    MySqlCommand cmd =
                        new MySqlCommand(agentSql, conn);

                    cmd.Parameters.AddWithValue("@ID", agent.ID);
                    cmd.Parameters.AddWithValue("@Name", agent.Name);
                    cmd.Parameters.AddWithValue("@Nummer", agent.Nummer);
                    cmd.Parameters.AddWithValue("@Specialitet", agent.Specialitet);
                    cmd.Parameters.AddWithValue("@Roll", agent.Roll);
                    cmd.Parameters.AddWithValue("@RegionKod", agent.RegionKod);

                    cmd.ExecuteNonQuery();

                    string lonSql = @"
                INSERT INTO AgentLon
                (AgentID, Lon)

                VALUES
                (@AgentID, @Lon)

                ON DUPLICATE KEY UPDATE
                    Lon = @Lon
            ";

                    MySqlCommand lonCmd =
                        new MySqlCommand(lonSql, conn);

                    lonCmd.Parameters.AddWithValue("@AgentID", agent.ID);
                    lonCmd.Parameters.AddWithValue("@Lon", agent.Lon);

                    lonCmd.ExecuteNonQuery();
                }

                return RedirectToAction("Index");
            }

            catch (MySqlException)
            {
                ViewBag.Error =
                    "Agentnummer finns redan.";

                return View(agent);
            }
        }

        // =========================================
        // DELETE
        // =========================================
        // Tar bort agent
        public IActionResult Delete(int id)
        {
            try
            {
                using (MySqlConnection conn =
                       new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                DELETE FROM Agenter
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
                    "Agenten kan inte tas bort eftersom den används i observationer.";

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}





