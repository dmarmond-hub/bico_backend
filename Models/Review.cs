using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Microsoft.AspNetCore.Authorization;

namespace Bico.Models
{
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("clienteId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ClienteId { get; set; }

        [BsonElement("prestadorId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PrestadorId { get; set; } = null!;

        [BsonElement("rating")] 
        public decimal Rating { get; set; }

        [BsonElement("comentario")]
        public string Comentario { get; set; }

        [BsonElement("criadoEm")]
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}
