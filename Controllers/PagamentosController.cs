using Bico.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Bico.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagamentosController : ControllerBase
    {
        private readonly IMongoCollection<Pagamento> _pagamentosCollection;

        public PagamentosController(
            IOptions<BicoDatabaseSettings> bicoDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bicoDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bicoDatabaseSettings.Value.DatabaseName);

            _pagamentosCollection = mongoDatabase.GetCollection<Pagamento>(
                bicoDatabaseSettings.Value.PagamentosCollectionName);
        }

        // ✅ GET: api/pagamentos
        [HttpGet]
        public async Task<ActionResult<List<Pagamento>>> GetAll()
        {
            var pagamentos = await _pagamentosCollection.Find(_ => true).ToListAsync();
            return Ok(pagamentos);
        }

        // ✅ GET: api/pagamentos/{id}
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Pagamento>> GetById(string id)
        {
            var pagamento = await _pagamentosCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (pagamento == null)
            {
                return NotFound("Usuário não encontrado.");
            }
            return Ok(pagamento);
        }

        // ✅ POST: api/pagamentos
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Pagamento newPagamento)
        {
            if (newPagamento == null)
            {
                return BadRequest("Dados inválidos.");
            }

            await _pagamentosCollection.InsertOneAsync(newPagamento);
            return CreatedAtAction(nameof(GetById), new { id = newPagamento.Id }, newPagamento);
        }

        // ✅ PUT: api/pagamentos/{id}
        [HttpPut("{id:length(24)}")]
        public async Task<ActionResult> Update(string id, [FromBody] Pagamento updatedPagamento)
        {
            var pagamento = await _pagamentosCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (pagamento == null)
            {
                return NotFound("Pagamento não encontrado.");
            }

            await _pagamentosCollection.ReplaceOneAsync(x => x.Id == id, updatedPagamento);
            return NoContent();
        }

        // ✅ DELETE: api/pagamentos/{id}
        [HttpDelete("{id:length(24)}")]
        public async Task<ActionResult> Delete(string id)
        {
            var pagamento = await _pagamentosCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (pagamento == null)
            {
                return NotFound("Pagamento não encontrado.");
            }

            await _pagamentosCollection.DeleteOneAsync(x => x.Id == id);
            return NoContent();
        }
    }
}

