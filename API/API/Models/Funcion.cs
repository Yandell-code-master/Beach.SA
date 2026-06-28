using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class Funcion
    {
        [Key]
        public int IdFuncion { get; set; }

        [Required]
        [StringLength(50)]
        public string? Codigo { get; set; }   // ej: "CLIENTES"

        [Required]
        [StringLength(100)]
        public string? Nombre { get; set; }    // ej: "Gestión de Clientes"

        [StringLength(100)]
        public string? Url { get; set; }       // ej: "/Cliente/Index"

        public int Orden { get; set; } = 0;

        public bool Estado { get; set; } = true;

        [JsonIgnore]
        public ICollection<RolFuncion>? RolFunciones { get; set; }
    }
}
