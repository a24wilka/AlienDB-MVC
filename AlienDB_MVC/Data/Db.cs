using MySql.Data.MySqlClient;

namespace AlienDB_MVC.Data
{
    // Klass för databaskoppling mot MySQL
    public class Db
    {
        // Sparar connection string från appsettings.json
        private readonly string _connectionString;

        // Konstruktor som hämtar connection string via IConfiguration
        public Db(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Returnerar en ny MySQL-anslutning
        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}