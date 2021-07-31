using System.Collections.Generic;

namespace SinoDbAPI.Models.Nightmares
{
    public class SkillMap
    {
        public IDictionary<string, SkillInfo> Colosseum { get; set; }

        public IDictionary<string, SkillInfo> Story { get; set; }
    }
}
