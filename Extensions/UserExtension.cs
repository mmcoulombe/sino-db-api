using SinoDbAPI.Jwt;
using SinoDbAPI.Models;


namespace SinoDbAPI.Extensions
{
    public static class UserExtension
    {
        public static Session ToSession(this User user, string token, System.TimeSpan expiration)
        {
            var now = System.DateTime.Now;
            var session = new Session()
            {
                UserId = user.Id,
                Username = user.Username,
                SessionToken = token,
                CreationDate = now,
                ExpirationDate = now.Add(expiration)
            };
            return session;
        }
    }
}