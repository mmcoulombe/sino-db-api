using SinoDbAPI.Models;

namespace SinoDbAPI.Payloads
{
    public class UserDetailResponse
    {
        public string Username { get; set; }

        public string[] Roles { get; set; }

        public static UserDetailResponse FromUser(User user)
        {
            return new UserDetailResponse()
            {
                Username = user.Username,
                Roles = user.Roles
            };
        }
    }
}
