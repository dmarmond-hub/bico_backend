using Bico.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Bico.Services
{
    public class UsuariosService
    {
        private readonly IMongoCollection<Usuario> _usuariosCollection;

        public UsuariosService(
            IOptions<BicoDatabaseSettings> bicoDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bicoDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bicoDatabaseSettings.Value.DatabaseName);

            _usuariosCollection = mongoDatabase.GetCollection<Usuario>(
                bicoDatabaseSettings.Value.UsuariosCollectionName);
        }

        public async Task<List<Usuario>> GetAll() =>
            await _usuariosCollection.Find(_ => true).ToListAsync();

        public async Task<Usuario?> GetById(string id) =>
            await _usuariosCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task Create(Usuario newUsuario) =>
            await _usuariosCollection.InsertOneAsync(newUsuario);

        public async Task Update(string id, Usuario updatedUsuario) =>
            await _usuariosCollection.ReplaceOneAsync(x => x.Id == id, updatedUsuario);

        public async Task Delete(string id) =>
            await _usuariosCollection.DeleteOneAsync(x => x.Id == id);
    }
}
