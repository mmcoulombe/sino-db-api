using SinoDbAPI.Models;

namespace SinoDbAPI.Payloads
{
    public class ColosseumResultRequest
    {

        public string Date { get; set; }

        public string GuildName { get; set; }

        public string GuildMaster { get; set; }

        public string Result { get; set; }

        public bool DuringGC { get; set; }

        public bool DuringSPColo { get; set; }

        public string StrategyUsed { get; set; }

        public ColosseumGuildStats OurStats { get; set; }

        public ColosseumGuildStats EnemyStats { get; set; }
    }
}