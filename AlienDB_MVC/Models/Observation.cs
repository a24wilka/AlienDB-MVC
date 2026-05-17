namespace AlienDB_MVC.Models
{
    // Modell för en observation
    public class Observation
    {
        public int ID { get; set; }

        public DateTime Datum { get; set; }

        public int Säkerhet { get; set; }

        public int Grad { get; set; }

        public int HandläggareID { get; set; }

        public int IncidentID { get; set; }

        public int AlienID { get; set; }
    }
}