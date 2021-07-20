using SinoDbAPI.Models;
using SinoDbAPI.Payloads;
using System;

namespace SinoDbAPI.Utils
{
    public class PayloadsConverter
    {
        public static ColosseumResult ToColosseumResult(ColosseumResultRequest request)
        {
            ColosseumResult result = new ColosseumResult();
            result.Date = request.Date;
            result.GuildName = request.GuildName;
            result.GuildMaster = request.GuildMaster;
            result.Result = (ColosseumResult.FightResult)Enum.Parse(typeof(ColosseumResult.FightResult), request.Result.ToUpper());
            result.DuringGC = request.DuringGC;
            result.OurStats = request.OurStats;
            result.EnemyStats = request.EnemyStats;

            return result;
        }
    }
}