using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Bico.Models
{
    public class Like
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("prestadorId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PrestadorId { get; set; } = null!;

        [BsonElement("clienteId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ClienteId { get; set; }

        [BsonElement("isLike")]
        public bool IsLike { get; set; }  // true para like, false para dislike
    }
}
