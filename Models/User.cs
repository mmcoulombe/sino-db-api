namespace SinoDbAPI.Models
{
    public class User
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string[] Roles { get; set; }

        public string Password { get; set; }

        static public User FromUsernamePassword(string username, string password)
        {
            User u = new User()
            {
                Username = username,
                Password = password,
                Roles = new string[0]
            };

            return u;
        }
    }
}