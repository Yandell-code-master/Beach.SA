using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class RolFuncion
    {
        [Key]
        public int IdRolFuncion { get; set; }

        [Required] public int IdRol { get; set; }
        [Required] public int IdFuncion { get; set; }

        [ForeignKey("IdRol")]
        [JsonIgnore]
        public Rol? Rol { get; set; }

        [ForeignKey("IdFuncion")]
        [JsonIgnore]
        public Funcion? Funcion { get; set; }
    }
}