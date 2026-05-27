using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class Cliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(20, ErrorMessage = "La cédula no puede colgarse de más de 20 caracteres")]
        public string Cedula { get; set; }

        [StringLength(10, ErrorMessage = "El tipo de cédula no puede exceder 10 caracteres.")]
        public string? TipoCedula { get; set; }

        [StringLength(100, ErrorMessage = "El nombre completo no puede exceder 100 caracteres.")]
        public string? NombreCompleto { get; set; }

        [Phone(ErrorMessage = "Teléfono con formato inválido.")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres.")]
        public string? Telefono { get; set; }

        [StringLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres.")]
        public string? Direccion { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Email con formato inválido.")]
        [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres.")]
        public string? Email { get; set; }
    }
}
