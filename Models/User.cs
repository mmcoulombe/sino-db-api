using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace SinoDbAPI.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Username { get; set; }

        public string[] Roles { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        static public User FromUsernamePassword(string username, string password)
        {
            User u = new User();
            u.Username = username;
            u.Password = password;
            u.Roles = new string[0];
            return u;
        }
    }
}