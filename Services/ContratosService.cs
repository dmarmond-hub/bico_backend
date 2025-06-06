using Bico.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Bico.Services
{
    public class ContratosService
    {
        private readonly IMongoCollection<Contrato> _contratosCollection;

        public ContratosService(
            IOptions<BicoDatabaseSettings> bicoDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bicoDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bicoDatabaseSettings.Value.DatabaseName);

            _contratosCollection = mongoDatabase.GetCollection<Contrato>(
                bicoDatabaseSettings.Value.ContratosCollectionName);
        }

        public async Task<List<Contrato>> GetAll() =>
            await _contratosCollection.Find(_ => true).ToListAsync();

        public async Task<Contrato?> GetById(string id) =>
            await _contratosCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task Create(Contrato newContrato) =>
            await _contratosCollection.InsertOneAsync(newContrato);

        public async Task Update(string id, Contrato updatedContrato) =>
            await _contratosCollection.ReplaceOneAsync(x => x.Id == id, updatedContrato);

        public async Task Delete(string id) =>
            await _contratosCollection.DeleteOneAsync(x => x.Id == id);
    }
}
