namespace Bico.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

    public class Usuario
    {
    [BsonId]// chave primária do documento
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("Nome")]
        public string Nome { get; set; }

    [Required]
    [BsonElement("email")]
        public string Email { get; set; }

    [BsonElement("CPF")]
    public string? CPF { get; set; }

    [BsonElement("telefone")] 
        public string? Telefone { get; set; }

    [Required]
    [BsonElement("password")]
        public string Password { get; set; }

    [Required]
    [BsonElement("tipo")]
        [BsonRepresentation(BsonType.String)] // Armazena o enum como string no MongoDB
        public UsuarioTipo Tipo { get; set; }


        [BsonElement("usuario")]
        public string? UsuarioNome { get; set; }

    [BsonElement("fotoUrl")]
    public string? FotoUrl { get; set; }


    [BsonElement("criadoEm")]
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        [BsonElement("atualizadoEm")]
        public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;


}

public enum UsuarioTipo
{
    Cliente,
    Prestador, 
    Admin
}


