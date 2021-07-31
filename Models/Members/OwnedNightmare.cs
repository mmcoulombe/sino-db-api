using SinoDbAPI.Models.Nightmares;

namespace SinoDbAPI.Models.Members
{
    public class OwnedNightmare
    {

        public string Name { get; set; }

        public SkillMap Skills { get; set; }

        public string IconName { get; set; }

        public string Id { get; set; }

        public bool Used { get; set; }

        public string Rarity { get; set; }
    }
}
