using MongoDB.Driver;
using SinoDbAPI.Models;
using SinoDbAPI.Settings;
using System.Collections.Generic;
using System.Linq;

namespace SinoDbAPI.Services
{
    public interface IColosseumResultService
    {
        List<ColosseumResult> Get();
        ColosseumResult GetById(string id);
        List<ColosseumResult> GetByGuildName(string guildName);
        ColosseumResult Create(ColosseumResult newResult);
        void Update(string id, ColosseumResult updatedResult);
        void Remove(string id);
    }

    public class ColosseumResultService : IColosseumResultService
    {
        private readonly IMongoCollection<ColosseumResult> _results;

        public ColosseumResultService(ISinoDataBaseSettings settings, IMongoClient client)
        {
            var database = client.GetDatabase(settings.DatabaseName);
            _results = database.GetCollection<ColosseumResult>(settings.ColosseumResultCollectionName);
        }

        public List<ColosseumResult> Get() =>
            _results.Find(elem => true).ToList();

        public ColosseumResult GetById(string id) =>
            _results.Find(elem => elem.Id.Equals(id)).FirstOrDefault();

        public List<ColosseumResult> GetByGuildName(string guildName) =>
            _results.Find(elem => elem.GuildName.Equals(guildName)).ToList();

        public ColosseumResult Create(ColosseumResult newResult)
        {
            _results.InsertOne(newResult);
            return newResult;
        }

        public void Update(string id, ColosseumResult updatedResult) =>
            _results.ReplaceOne(elem => elem.Id == id, updatedResult);

        public void Remove(string id) =>
            _results.DeleteOne(elem => elem.Id == id);
    }
}