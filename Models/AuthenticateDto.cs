using System.ComponentModel.DataAnnotations;

namespace Bico.Models
{
    public class AuthenticateDto
    {
        [Required]
        [EmailAddress]  // Validação para garantir um e-mail válido
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
