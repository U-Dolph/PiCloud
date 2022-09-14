namespace PiCloudDashboard.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public DateTime Featured { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}

