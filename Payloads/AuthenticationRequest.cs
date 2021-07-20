using System.ComponentModel.DataAnnotations;

namespace SinoDbAPI.Payloads
{
    public class AuthenticationRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
