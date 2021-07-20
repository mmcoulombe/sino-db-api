namespace SinoDbAPI.Settings
{
    public interface IAuthenticationSettings
    {
        string JwtSecret { get; set; }
        string BCryptSecret { get; set; }
    }

    public class AuthenticationSettings : IAuthenticationSettings
    {
        public string JwtSecret { get; set; }
        public string BCryptSecret { get; set; }
    }
}