using Core.Arango;
using Newtonsoft.Json;
using SinoDbAPI.Models.Nightmares;
using SinoDbAPI.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;

namespace SinoDbAPI.Services
{
    public interface INightmareService
    {
        Task<List<NightmareInfo>> Get();

        Task<NightmareInfo> GetById(string id);

        Task<List<NightmareInfo>> Find(string name);

        Task<string> Create(NightmareInfo newResult);

        Task Update(string id, NightmareInfo updatedResult);

        void Remove(string id);
    }

    public class NightmareService : INightmareService
    {
        private readonly IArangoContext _arangoContext;

        private readonly string _databaseName;
        private readonly string _collectionName;

        public NightmareService(ISinoDataBaseSettings settings, IArangoContext arangoContext)
        {
            _databaseName = settings.DatabaseName;
            _collectionName = settings.NightmareCollectionName;
            _arangoContext = arangoContext;
        }

        public async Task<List<NightmareInfo>> Get()
        {
            var query = string.Format("FOR result IN {0} RETURN result", _collectionName);
            var result = await _arangoContext.Query.ExecuteAsync<NightmareInfo>(_databaseName, query, null);
            return result.ToList();
        }

        public async Task<NightmareInfo> GetById(string id)
        {
            var query = string.Format("RETURN DOCUMENT('{0}/{1}')", _collectionName, id);
            var result = await _arangoContext.Query.ExecuteAsync<NightmareInfo>(_databaseName, query, null);
            return result.FirstOrDefault();
        }

        public async Task<List<NightmareInfo>> Find(string name)
        {
            var query = string.Format("FOR result IN {0} FILTER result.Name == '{1}' RETURN result", _collectionName, name);
            var result = await _arangoContext.Query.ExecuteAsync<NightmareInfo>(_databaseName, query, null);
            return result.ToList();
        }

        public async Task<string> Create(NightmareInfo newResult)
        {
            var result = await _arangoContext.Document.CreateAsync<NightmareInfo>(_databaseName, _collectionName, newResult);
            return result.Id;
        }

        public Task Update(string key, NightmareInfo updatedResult)
        {
            var jsonResult = JsonConvert.SerializeObject(updatedResult);
            var query = string.Format("REPLACE '{0}' WITH {{ {1} }} IN session_cache", key, jsonResult);

            return _arangoContext.Query.ExecuteAsync<Jwt.Session>(_databaseName, query, null);
        }

        public void Remove(string id)
        {
            _arangoContext.Document.DeleteAsync<NightmareInfo>(_databaseName, _collectionName, id);
        }
        
    }
}
