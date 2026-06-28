using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class Paquete
    {
        [Key]
        [JsonIgnore]
        public int IdPaquete { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(100, ErrorMessage = "La descripción no puede exceder 100 caracteres.")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio por noche es obligatorio.")]
        [Range(1, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
        public decimal PrecioPorNoche { get; set; }

        //Porcentaje de prima
        //Ejemplo:
        //45% = 0.45
        //35% = 0.35
        [Required(ErrorMessage = "La prima es obligatoria.")]
        [Range(0, 1, ErrorMessage = "La prima debe estar entre 0 y 1.")]
        public decimal Prima { get; set; }

        //Cantidad de meses para pagar el financiamiento
        [Required(ErrorMessage = "La cantidad de meses es obligatoria.")]
        [Range(1, 100, ErrorMessage = "Los meses deben ser mayores a 0.")]
        public int Meses { get; set; }

        //Estado del paquete -- true = activo -- false = inactivo
        public bool Estado { get; set; } = true;
    }
}
