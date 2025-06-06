using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Bico.Models
{
    public class Pagamento
    {
        [BsonId]// chave primária do documento
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("prestadorId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PrestadorId { get; set; }

        [BsonElement("clienteId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ClienteId { get; set; }

        [BsonElement("servicoId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ServicoId { get; set; }
       
        [BsonElement("contratoId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ContratoId { get; set; }

        [BsonElement("valor")]
        public double Valor { get; set; }

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)] // Armazena o enum como string no MongoDB
        public StatusPagamento Status { get; set; }

        [BsonElement("metodoPagamento")]
        [BsonRepresentation(BsonType.String)] // Armazena o enum como string no MongoDB
        public MetodoPagamento MetodoPagamento { get; set; }



        [BsonElement("criadoEm")]
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;


    }

    public enum StatusPagamento
    {
        falha,
        pendente,
        pago
    }

    public enum MetodoPagamento
    {
        cartão,
        PIX
    }
}
