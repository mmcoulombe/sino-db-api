namespace SinoDbAPI.Settings
{
    public interface ISinoDataBaseSettings
    {
        string ColosseumResultCollectionName { get; set; }

        string UserCollectionName { get; set; }

        string NightmareCollectionName { get; set; }

        string MemberCollectionNome { get; set; }

        string DatabaseName { get; set; }
    }

    public class SinoDataBaseSettings : ISinoDataBaseSettings
    {
        public string ColosseumResultCollectionName { get; set; }
        
        public string UserCollectionName { get; set; }

        public string NightmareCollectionName { get; set; }

        public string MemberCollectionNome { get; set; }


        public string DatabaseName { get; set; }
    }
}