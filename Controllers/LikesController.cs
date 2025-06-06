using Bico.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Bico.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly IMongoCollection<Like> _likesCollection;

        public LikesController(IOptions<BicoDatabaseSettings> bicoDatabaseSettings)
        {
            var mongoClient = new MongoClient(bicoDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(bicoDatabaseSettings.Value.DatabaseName);
            _likesCollection = mongoDatabase.GetCollection<Like>(bicoDatabaseSettings.Value.LikesCollectionName);
        }

        [HttpGet("{prestadorId}")]
        public async Task<ActionResult<object>> GetLikesDislikesCount(string prestadorId)
        {
            var likesCount = await _likesCollection.CountDocumentsAsync(like => like.PrestadorId == prestadorId && like.IsLike);
            var dislikesCount = await _likesCollection.CountDocumentsAsync(like => like.PrestadorId == prestadorId && !like.IsLike);
            return Ok(new { likes = likesCount, dislikes = dislikesCount });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddLikeOrDislike([FromBody] Like newLike)
        {
            var existing = await _likesCollection.Find(like => like.PrestadorId == newLike.PrestadorId && like.ClienteId == newLike.ClienteId).FirstOrDefaultAsync();
            if (existing != null)
            {
                if (existing.IsLike == newLike.IsLike)
                {
                    return BadRequest("Like or dislike already exists.");
                }
                else
                {
                    // Update existing to new value
                    var update = Builders<Like>.Update.Set(l => l.IsLike, newLike.IsLike);
                    await _likesCollection.UpdateOneAsync(like => like.Id == existing.Id, update);
                    return Ok();
                }
            }

            await _likesCollection.InsertOneAsync(newLike);
            return Ok();
        }

        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> RemoveLikeOrDislike([FromBody] Like likeToRemove)
        {
            var result = await _likesCollection.DeleteOneAsync(like => like.PrestadorId == likeToRemove.PrestadorId && like.ClienteId == likeToRemove.ClienteId && like.IsLike == likeToRemove.IsLike);
            if (result.DeletedCount == 0)
            {
                return NotFound("Like or dislike not found.");
            }
            return NoContent();
        }
    }
}
