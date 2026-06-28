using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required]
        [StringLength(256)]
        [JsonIgnore] // nunca se devuelve la contraseña
        public string? Password { get; set; }

        [Required]
        public int IdRol { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public bool Estado { get; set; } = true;

        [ForeignKey("IdRol")]
        public Rol? Rol { get; set; }
    }
}
