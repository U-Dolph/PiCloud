namespace PiCloudDashboard.Models
{
    public class JWT
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }

        public bool Result { get; set; }
        public List<string> Errors { get; set; }
    }
}
