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
    public class ReviewsController : ControllerBase
    {
        private readonly IMongoCollection<Review> _reviewsCollection;

        public ReviewsController(
            IOptions<BicoDatabaseSettings> bicoDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bicoDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bicoDatabaseSettings.Value.DatabaseName);

            _reviewsCollection = mongoDatabase.GetCollection<Review>(
                bicoDatabaseSettings.Value.ReviewsCollectionName);
        }

        // ✅ GET: api/reviews
        //[Authorize(Roles = "Admin")] // apenas usuários do tipo Admin podem acessar esse método
        [HttpGet]
        public async Task<ActionResult<List<Review>>> GetAll([FromQuery] string? prestadorId)
        {
            FilterDefinition<Review> filter = FilterDefinition<Review>.Empty;

            if (!string.IsNullOrEmpty(prestadorId))
            {
                filter = Builders<Review>.Filter.Eq(r => r.PrestadorId, prestadorId);
            }

            var reviews = await _reviewsCollection.Find(filter).ToListAsync();
            return Ok(reviews);
        }

        // ✅ GET: api/reviews/{id}
        //[Authorize(Roles = "Admin")] // ajustar roles se necessário
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Review>> GetById(string id)
        {
            var review = await _reviewsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (review == null)
            {
                return NotFound("Review não encontrada.");
            }

            return Ok(review);
        }

        // ✅ POST: api/reviews
        //[Authorize(Roles = "Cliente, Prestador, Admin")]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Review newReview)
        {
            if (newReview == null)
            {
                return BadRequest("Dados inválidos.");
            }

            await _reviewsCollection.InsertOneAsync(newReview);
            return CreatedAtAction(nameof(GetById), new { id = newReview.Id }, newReview);
        }

        // ✅ PUT: api/reviews/{id}
        //[Authorize(Roles = "Cliente, Prestador")] // apenas usuários dos tipos Cliente e Prestador podem acessar esse método
        [HttpPut("{id:length(24)}")]
        public async Task<ActionResult> Update(string id, [FromBody] Review updatedReview)
        {
            var review = await _reviewsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (review == null)
            {
                return NotFound("Review não encontrada.");
            }

            await _reviewsCollection.ReplaceOneAsync(x => x.Id == id, updatedReview);
            return NoContent();
        }

        // ✅ DELETE: api/reviews/{id}
        [Authorize(Roles = "Cliente, Prestador, Admin")]
        [HttpDelete("{id:length(24)}")]
        public async Task<ActionResult> Delete(string id)
        {
            var review = await _reviewsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (review == null)
            {
                return NotFound("Review não encontrada.");
            }

            await _reviewsCollection.DeleteOneAsync(x => x.Id == id);
            return NoContent();
        }
    }
}
