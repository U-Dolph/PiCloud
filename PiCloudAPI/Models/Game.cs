using System.ComponentModel.DataAnnotations;

namespace PiCloud.Models
{
    public class Game
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string Version { get; set; } = string.Empty;
        [Required]
        public DateTime Featured { get; set; }
        [Required]
        public DateTime LastUpdated { get; set; }
    }
}
