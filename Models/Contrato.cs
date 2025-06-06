using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Bico.Models
{
    public class Contrato
    {
        [BsonId]// chave primária do documento
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("clienteId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ClienteId { get; set; }
        
        [BsonElement("servicoId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ServicoId { get; set; }

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)] // Armazena o enum como string no MongoDB
        public StatusTipo Status { get; set; }

        [BsonElement("criadoEm")]
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;


    }

    public enum StatusTipo
    {
        pendente, 
        andamento, 
        finalizado
    }
}
