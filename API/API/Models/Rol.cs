using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class Rol
    {
        [Key]
        public int IdRol { get; set; }

        [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
        [StringLength(50)]
        public string? Nombre { get; set; }

        [StringLength(200)]
        public string? Descripcion { get; set; }

        public bool Estado { get; set; } = true;

        [JsonIgnore]
        public ICollection<Usuario>? Usuarios { get; set; }

        [JsonIgnore]
        public ICollection<RolFuncion>? RolFunciones { get; set; }
    }
}