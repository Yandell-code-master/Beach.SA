using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Cliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Range(1, int.MaxValue, ErrorMessage = "La cédula debe ser un número positivo.")]
        public int Cedula { get; set; }

        [Required(ErrorMessage = "El tipo de cédula es obligatorio.")]
        [StringLength(10, ErrorMessage = "El tipo de cédula no puede exceder 10 caracteres.")]
        public string? TipoCedula { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
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
