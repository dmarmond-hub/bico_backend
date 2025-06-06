using Bico.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Bico.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IMongoCollection<Usuario> _usuariosCollection;

        public UsuariosController(
            IOptions<BicoDatabaseSettings> bicoDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bicoDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bicoDatabaseSettings.Value.DatabaseName);

            _usuariosCollection = mongoDatabase.GetCollection<Usuario>(
                bicoDatabaseSettings.Value.UsuariosCollectionName);
        }

        // ✅ GET: api/usuarios
        //[Authorize(Roles = "Admin")] // apenas o usuário do tipo admin pode acessar esse método
        [HttpGet]
        public async Task<ActionResult<List<Usuario>>> GetAll()
        {
            var usuarios = await _usuariosCollection.Find(_ => true).ToListAsync();
            return Ok(usuarios);
        }

        // ✅ GET: api/usuarios/{id}
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Usuario>> GetById(string id)
        {
            var usuario = await _usuariosCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }
            return Ok(usuario);
        }

        // ✅ GET: api/usuarios/me - Retorna dados do usuário autenticado
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult> GetMe()
        {
            // Extrai o ID do usuário do token JWT
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Token inválido");

            // Busca o usuário no banco de dados
            var usuario = await _usuariosCollection.Find(x => x.Id == userId).FirstOrDefaultAsync();

            if (usuario == null)
                return NotFound("Usuário não encontrado");

            // Retorna apenas os dados necessários
            return Ok(new
            {
                usuario.Id,
                usuario.Nome,
                usuario.Email,
                Tipo = usuario.Tipo.ToString() // Convertendo o enum para string diretamente
            });
        }

        // ✅ POST: api/usuarios
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Usuario newUsuario)
        {
            if (newUsuario == null)
            {
                return BadRequest("Dados inválidos.");
            }

            await _usuariosCollection.InsertOneAsync(newUsuario);
            return CreatedAtAction(nameof(GetById), new { id = newUsuario.Id }, newUsuario);
        }

        // ✅ PUT: api/usuarios/{id}
        //[Authorize] // necessita de autenticação
        [HttpPut("{id:length(24)}")]
        public async Task<ActionResult> Update(string id, [FromBody] Usuario updatedUsuario)
        {
            var usuario = await _usuariosCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            await _usuariosCollection.ReplaceOneAsync(x => x.Id == id, updatedUsuario);
            return NoContent();
        }

        // ✅ DELETE: api/usuarios/{id}
        //[Authorize] // necessita de autenticação
        [HttpDelete("{id:length(24)}")]
        public async Task<ActionResult> Delete(string id)
        {
            var usuario = await _usuariosCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            await _usuariosCollection.DeleteOneAsync(x => x.Id == id);
            return NoContent();
        }

        // ✅ Authenticate: api/usuarios/authenticate
        [HttpPost("authenticate")]
        public async Task<ActionResult> Authenticate([FromBody] AuthenticateDto model)
        {
            var usuarioDb = await _usuariosCollection.Find(x => x.Email == model.Email).FirstOrDefaultAsync();

            if (usuarioDb == null || usuarioDb.Password != model.Password)
            {
                return Unauthorized("Usuário e/ou Senha incorretos.");
            }

            var jwt = GenerateJwtToken(usuarioDb);

            return Ok(new { Message = "Autenticação bem-sucedida", jwtToken = jwt });
        }

        private string GenerateJwtToken(Usuario model)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("Ry74cBQva5dThwbwchR9jhbtRFnJxWSZ");
            var claims = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, model.Id.ToString()),
                new Claim(ClaimTypes.Role, model.Tipo.ToString())
            });
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpPost("{id:length(24)}/photo")]
        public async Task<IActionResult> UploadPhoto(string id, IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
            {
                return BadRequest("Nenhuma foto enviada.");
            }

            var usuario = await _usuariosCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            var photoStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "users");
            if (!Directory.Exists(photoStoragePath))
            {
                Directory.CreateDirectory(photoStoragePath);
            }

            var fileExtension = Path.GetExtension(photo.FileName);
            var fileName = $"{id}_{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(photoStoragePath, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                var photoUrl = $"/uploads/users/{fileName}";
                usuario.FotoUrl = photoUrl;

                await _usuariosCollection.ReplaceOneAsync(x => x.Id == id, usuario);

                return Ok(new { PhotoUrl = photoUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao salvar a foto: {ex.Message}");
            }
        }

    }
}
