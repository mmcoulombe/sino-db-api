using System.Collections.Generic;

namespace SinoDbAPI.Models.Members
{
    public class MemberInfo
    {
        public string Name { get; set; }

        public IEnumerable<OwnedNightmare> Nightmares;
    }
}
