using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using System.Text.Json.Serialization;

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

        [JsonIgnore]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [JsonPropertyName("Date")]
        public string Date { get; set; }
        
        [JsonPropertyName("GuildName")]
        public string GuildName { get; set; }
        
        [JsonPropertyName("GuildMaster")]
        public string GuildMaster { get; set; }

        [JsonPropertyName("Result")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public FightResult Result { get; set; }

        [JsonPropertyName("DuringGC")]
        public bool DuringGC { get; set; }

        [JsonPropertyName("DuringSPColor")]
        public bool DuringSPColo { get; set; }

        [JsonPropertyName("StrategyUsed")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public FightStrategy StrategyUsed { get; set; }

        [JsonPropertyName("OurStats")]
        public ColosseumGuildStats OurStats { get; set; }

        [JsonPropertyName("EnemyStats")]
        public ColosseumGuildStats EnemyStats { get; set; }
    }
}