using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Servico
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("prestadorId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string PrestadorId { get; set; } = null!; // ID do usuário prestador

    [BsonElement("titulo")]
    public string Titulo { get; set; } = null!;

    [BsonElement("descricao")]
    public string Descricao { get; set; } = null!;

    [BsonElement("categoria")]
    public string Categoria { get; set; } = null!;

    [BsonElement("preco")]
    [BsonIgnoreIfNull] // Torna o campo opcional
    public double? Preco { get; set; }

    [BsonElement("localizacao")]
    public string Localizacao { get; set; } = null!;

    [BsonElement("imagens")]
    [BsonIgnoreIfNull] // Torna o campo opcional
    public List<string>? Imagens { get; set; }

    [BsonElement("videos")]
    [BsonIgnoreIfNull] // Torna o campo opcional
    public List<string>? Videos { get; set; }

    [BsonElement("criadoEm")]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    [BsonElement("atualizadoEm")]
    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;
}
