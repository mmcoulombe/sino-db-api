using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SinoDbAPI.Models
{
    public class ColosseumResult
    {
        public enum FightResult
        {
            Won,
            Lost
        }

        public enum FightStrategy
        {
            Combo,
            Rush
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Date { get; set; }
        
        public string GuildName { get; set; }
        
        public string GuildMaster { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public FightResult Result { get; set; }

        public bool DuringGC { get; set; }

        public bool DuringSPColo { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public FightStrategy StrategyUsed { get; set; }

        public ColosseumGuildStats OurStats { get; set; }

        public ColosseumGuildStats EnemyStats { get; set; }
    }
}