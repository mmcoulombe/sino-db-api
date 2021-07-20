namespace SinoDbAPI.Settings
{
    public interface ISinoDataBaseSettings
    {
        string ColosseumResultCollectionName { get; set; }
        string UserCollectionName { get; set; }
        string DatabaseName { get; set; }
    }

    public class SinoDataBaseSettings : ISinoDataBaseSettings
    {
        public string ColosseumResultCollectionName { get; set; }
        public string UserCollectionName { get; set; }
        public string DatabaseName { get; set; }
    }
}