using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class UsuarioDTO
    {
        [Required][EmailAddress] public string? Email { get; set; }
        [Required] public string? Password { get; set; }
        public bool Estado { get; set; } = true;
    }
}
