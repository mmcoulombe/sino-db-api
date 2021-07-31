using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

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
            Rush,
            Auto
        }

        [JsonIgnore]
        public string Id { get; set; }

        [JsonProperty("Date")]
        public string Date { get; set; }
        
        [JsonProperty("GuildName")]
        public string GuildName { get; set; }
        
        [JsonProperty("GuildMaster")]
        public string GuildMaster { get; set; }

        [JsonProperty("Result")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FightResult Result { get; set; }

        [JsonProperty("DuringGC")]
        public bool DuringGC { get; set; }

        [JsonProperty("DuringSPColor")]
        public bool DuringSPColo { get; set; }

        [JsonProperty("StrategyUsed")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FightStrategy StrategyUsed { get; set; }

        [JsonProperty("OurStats")]
        public ColosseumGuildStats OurStats { get; set; }

        [JsonProperty("EnemyStats")]
        public ColosseumGuildStats EnemyStats { get; set; }
    }
}