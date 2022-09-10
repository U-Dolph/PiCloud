using System.ComponentModel.DataAnnotations;

namespace PiCloud.Data
{
    public class UserRegistrationDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
