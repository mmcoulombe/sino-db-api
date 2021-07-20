using SinoDbAPI.Models;

namespace SinoDbAPI.Payloads
{
    public class AuthenticationResponse
    {
        public string Id { get; set; }
        
        public string Username { get; set; }
        
        public string Token { get; set; }

        public AuthenticationResponse(User user, string token)
        {
            Id = user.Id;
            Username = user.Username;
            Token = token;
        }
    }
}