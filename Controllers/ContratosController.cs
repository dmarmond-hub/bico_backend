using Bico.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Bico.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContratosController : ControllerBase
    {

        private readonly IMongoCollection<Contrato> _contratosCollection;
        public ContratosController(IOptions<BicoDatabaseSettings> bicoDatabaseSettings)
        {
            var mongoClient = new MongoClient(bicoDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(bicoDatabaseSettings.Value.DatabaseName);
            _contratosCollection = mongoDatabase.GetCollection<Contrato>(bicoDatabaseSettings.Value.ContratosCollectionName);
        }


        // ✅ GET: api/contratos
        [HttpGet]
        public async Task<ActionResult<List<Contrato>>> GetAll()
        {
            var contratos = await _contratosCollection.Find(_ => true).ToListAsync();
            return Ok(contratos);
        }

        // ✅ GET: api/contratos/{id}
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Contrato>> GetById(string id)
        {
            var contrato = await _contratosCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (contrato == null)
            {
                return NotFound("Contrato não encontrado.");
            }
            return Ok(contrato);
        }

        // ✅ POST: api/contratos
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Contrato newContrato)
        {
            if (newContrato == null)
            {
                return BadRequest("Dados inválidos.");
            }

            await _contratosCollection.InsertOneAsync(newContrato);
            return CreatedAtAction(nameof(GetById), new { id = newContrato.Id }, newContrato) ;
            
        }

        // ✅ PUT: api/contratos/{id}
        [HttpPut("{id:length(24)}")]
        public async Task<ActionResult> Update(string id, [FromBody] Contrato updatedContrato)
        {
            var contrato = await _contratosCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (contrato == null)
            {
                return NotFound("Contrato não encontrado.");
            }

            await _contratosCollection.ReplaceOneAsync(x => x.Id == id, updatedContrato);
            return NoContent();
        }

        // ✅ DELETE: api/contratos/{id}
        [HttpDelete("{id:length(24)}")]
        public async Task<ActionResult> Delete(string id)
        {
            var contrato = await _contratosCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (contrato == null)
            {
                return NotFound("Contrato não encontrado.");
            }

            await _contratosCollection.DeleteOneAsync(x => x.Id == id);
            return NoContent();
        }

        public static implicit operator ContratosController(ServicosController v)
        {
            throw new NotImplementedException();
        }
    }
}
