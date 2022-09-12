namespace PiCloud.Configurations
{
    public class JwtConfig
    {
        public string Key { get; set; }
        public TimeSpan ExpiryTimeFrame { get; set; }
    }
}
