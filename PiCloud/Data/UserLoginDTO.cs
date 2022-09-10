using System.ComponentModel.DataAnnotations;

namespace PiCloud.Data
{
    public class UserLoginDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
