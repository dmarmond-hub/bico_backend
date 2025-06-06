using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace Bico.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordRecoveryController : ControllerBase
    {
        private readonly IMongoCollection<Usuario> _usuariosCollection;
        private readonly SmtpClient _smtpClient;
        private readonly string _fromEmail = "suportebico@gmail.com";
        private readonly string _resetLinkBaseUrl = "http://localhost:3000/recuperar-senha.html?token=";

        public PasswordRecoveryController(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("Bico");
            _usuariosCollection = database.GetCollection<Usuario>("usuario");

            _smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_fromEmail, "bico2025"), // Use uma senha de app aqui
                EnableSsl = true
            };
        }

        public class Usuario
        {
            public ObjectId Id { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string ResetToken { get; set; }
            public DateTime? ResetTokenExpiration { get; set; }
        }

        public class RequestRecoveryModel
        {
            public string Email { get; set; }
        }

        public class ResetPasswordModel
        {
            public string Token { get; set; }
            public string NewPassword { get; set; }
        }

        // POST api/passwordrecovery/request
        [HttpPost("request")]
        public async Task<IActionResult> RequestPasswordRecovery([FromBody] RequestRecoveryModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
                return BadRequest(new { message = "Email é obrigatório." });

            var user = await _usuariosCollection.Find(u => u.Email == model.Email).FirstOrDefaultAsync();
            if (user == null)
                return BadRequest(new { message = "Usuário não encontrado." });

            var token = GenerateToken();
            var expiration = DateTime.UtcNow.AddHours(1);

            var update = Builders<Usuario>.Update
                .Set(u => u.ResetToken, token)
                .Set(u => u.ResetTokenExpiration, expiration);

            await _usuariosCollection.UpdateOneAsync(u => u.Id == user.Id, update);

            var resetLink = _resetLinkBaseUrl + token;
            var mailMessage = new MailMessage(_fromEmail, user.Email)
            {
                Subject = "Recuperação de senha - Bico",
                Body = $"Olá,\n\nClique no link para redefinir sua senha:\n{resetLink}\n\nEste link expira em 1 hora.",
                IsBodyHtml = false
            };

            try
            {
                await _smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Erro ao enviar email." });
            }

            return Ok(new { message = "Email de recuperação enviado." });
        }

        // POST api/passwordrecovery/reset
        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Token) || string.IsNullOrWhiteSpace(model.NewPassword))
                return BadRequest(new { message = "Token e nova senha são obrigatórios." });

            var user = await _usuariosCollection.Find(u => u.ResetToken == model.Token).FirstOrDefaultAsync();

            if (user == null || user.ResetTokenExpiration == null || user.ResetTokenExpiration < DateTime.UtcNow)
            {
                return BadRequest(new { message = "Token inválido ou expirado." });
            }


            user.Password = ComputeSha256Hash(model.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpiration = null;

            await _usuariosCollection.ReplaceOneAsync(u => u.Id == user.Id, user);

            return Ok(new { message = "Senha redefinida com sucesso." });
        }

        private string ComputeSha256Hash(string newPassword)
        {
            throw new NotImplementedException();
        }

        private string GenerateToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}
