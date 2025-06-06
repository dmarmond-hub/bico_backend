using Bico.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Bico.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicosController : ControllerBase
    {
        private readonly IMongoCollection<Servico> _servicosCollection;

        public ServicosController(IOptions<BicoDatabaseSettings> bicoDatabaseSettings)
        {
            var mongoClient = new MongoClient(bicoDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(bicoDatabaseSettings.Value.DatabaseName);
            _servicosCollection = mongoDatabase.GetCollection<Servico>(bicoDatabaseSettings.Value.ServicosCollectionName);
        }

        // ✅ GET: api/servicos?prestadorId={prestadorId}
        [HttpGet]
        public async Task<ActionResult<List<Servico>>> GetAll([FromQuery] string prestadorId = null)
        {
            FilterDefinition<Servico> filter = Builders<Servico>.Filter.Empty;

            if (!string.IsNullOrEmpty(prestadorId))
            {
                filter = Builders<Servico>.Filter.Eq(s => s.PrestadorId, prestadorId);
            }

            var servicos = await _servicosCollection.Find(filter).ToListAsync();
            return Ok(servicos);
        }

        // ✅ GET: api/servicos/{id}
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Servico>> GetServiceById(string id)
        {
            var servico = await _servicosCollection.Find(s => s.Id == id).FirstOrDefaultAsync();
            if (servico == null)
            {
                return NotFound("Serviço não encontrado.");
            }
            return Ok(servico);
        }

        // ✅ POST: api/servicos
        [Authorize(Roles = "Prestador")] // apenas o usuário do tipo prestador pode acessar esse método
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Servico newServico)
        {
            if (newServico == null)
            {
                return BadRequest("Dados inválidos.");
            }

            await _servicosCollection.InsertOneAsync(newServico);
            return CreatedAtAction(nameof(GetServiceById), new { id = newServico.Id }, newServico);
        }

        // ✅ PUT: api/servicos/{id}
        [Authorize(Roles = "Prestador")] // apenas o usuário do tipo prestador pode acessar esse método
        [HttpPut("{id:length(24)}")]
        public async Task<ActionResult> Update(string id, [FromBody] Servico updatedServico)
        {
            var servico = await _servicosCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (servico == null)
            {
                return NotFound("Serviço não encontrado.");
            }

            await _servicosCollection.ReplaceOneAsync(x => x.Id == id, updatedServico);
            return NoContent();
        }

        // ✅ DELETE: api/servicos/{id}
        [Authorize(Roles = "Prestador, Admin")] // apenas o usuário do tipo prestador e admin podem acessar esse método
        [HttpDelete("{id:length(24)}")]
        public async Task<ActionResult> Delete(string id)
        {
            var servico = await _servicosCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (servico == null)
            {
                return NotFound("Serviço não encontrado.");
            }

            await _servicosCollection.DeleteOneAsync(x => x.Id == id);
            return NoContent();
        }

        // ✅ POST: api/servicos/{id}/upload-image
        //[Authorize(Roles = "Prestador")] // apenas o usuário do tipo prestador pode acessar o upload de imagem
        [HttpPost("{id:length(24)}/upload-image")]
        public async Task<IActionResult> UploadServiceImage(string id, IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("Nenhuma imagem enviada.");
            }

            var servico = await _servicosCollection.Find(s => s.Id == id).FirstOrDefaultAsync();
            if (servico == null)
            {
                return NotFound("Serviço não encontrado.");
            }

            var imageStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "servicos");
            if (!Directory.Exists(imageStoragePath))
            {
                Directory.CreateDirectory(imageStoragePath);
            }

            var fileExtension = Path.GetExtension(image.FileName);
            var fileName = $"{id}_{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(imageStoragePath, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                var imageUrl = $"/uploads/servicos/{fileName}";

                if (servico.Imagens == null)
                {
                    servico.Imagens = new List<string>();
                }

                servico.Imagens.Add(imageUrl);

                await _servicosCollection.ReplaceOneAsync(s => s.Id == id, servico);

                return Ok(new { ImageUrl = imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao salvar a imagem: {ex.Message}");
            }
        }
    }
}
