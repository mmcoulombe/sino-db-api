using Newtonsoft.Json;
using System.Collections.Generic;

namespace SinoDbAPI.Models.Nightmares
{
    public class NightmareInfo
    {
        [JsonIgnore]
        public string Id { get; set; }

        public string Name { get; set; }

        // todo: use an enum
        public IEnumerable<string> Rarity { get; set; }

        public SkillMap Skills { get; set; }

        public string IconName { get; set; }
    }
}
