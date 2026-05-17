namespace AlienDB_MVC.Models
{
    // Modell för aktivitetslogg på dashboarden
    public class ActivityLog
    {
        public string? Händelse { get; set; }

        public string? TabellNamn { get; set; }

        public DateTime Tid { get; set; }
    }
}