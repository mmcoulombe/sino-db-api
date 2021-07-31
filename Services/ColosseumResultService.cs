using Core.Arango;

using SinoDbAPI.Models;
using SinoDbAPI.Settings;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SinoDbAPI.Services
{
    public interface IColosseumResultService
    {
        Task<List<ColosseumResult>> Get(int? limit);
        Task<ColosseumResult> GetById(string id);
        Task<List<ColosseumResult>> GetByGuildName(string guildName);
        Task<string> Create(ColosseumResult newResult);
        void Update(string id, ColosseumResult updatedResult);
        void Remove(string id);
    }

    public class ColosseumResultService : IColosseumResultService
    {
        private readonly IArangoContext _arangoContext;

        private readonly string _databaseName;
        private readonly string _collectionName;

        public ColosseumResultService(ISinoDataBaseSettings settings, IArangoContext arangoContext)
        {
            _arangoContext = arangoContext;
            _databaseName = settings.DatabaseName;
            _collectionName = settings.ColosseumResultCollectionName;
        }

        public async Task<List<ColosseumResult>> Get(int? limit)
        {
            var limitStr = (!limit.HasValue || limit <= 0) ? "" : $"LIMIT {limit}";

            var query = string.Format("FOR result IN {0} {1} RETURN result", _collectionName, limitStr);
            var result = await _arangoContext.Query.ExecuteAsync<ColosseumResult>(_databaseName, query, null);
            return result.ToList();
        }

        public async Task<ColosseumResult> GetById(string id)
        {
            var query = string.Format("RETURN DOCUMENT('{0}/{1}')", _collectionName, id);
            var result = await _arangoContext.Query.ExecuteAsync<ColosseumResult>(_databaseName, query, null) ;
            return result.FirstOrDefault();
        }

        public async Task<List<ColosseumResult>> GetByGuildName(string guildName)
        {

            var query = string.Format("FOR result IN {0} FILTER result.GuildName == '{1}' RETURN result", _collectionName, guildName);
            var result = await _arangoContext.Query.ExecuteAsync<ColosseumResult>(_databaseName, query, null);
            return result.ToList();
        }

        public async Task<string> Create(ColosseumResult newResult)
        {
            var result = await _arangoContext.Document.CreateAsync<ColosseumResult>(_databaseName, _collectionName, newResult);
            return result.Id;
        }

        public void Update(string id, ColosseumResult updatedResult)
        {
            return;
        }


        public void Remove(string id)
        {
            _arangoContext.Document.DeleteAsync<ColosseumResult>(_databaseName, _collectionName, id);
        }
    }
}