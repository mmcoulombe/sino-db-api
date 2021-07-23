using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using SinoDbAPI.Serialization;

namespace SinoDbAPI.Jwt
{
    public class Session
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string SessionToken { get; set; }

        [JsonConverter(typeof(DateTimeJsonConverter))]
        public DateTime CreationDate { get; set; }

        [JsonConverter(typeof(DateTimeJsonConverter))]
        public DateTime ExpirationDate { get; set; }
    }
}
