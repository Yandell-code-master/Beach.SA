using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApp_MVC.Models
{
    public class Reservacion
    {
        [Key]
        public int IdReservacion { get; set; }

        [Required(ErrorMessage = "La cedula del cliente es obligatorio.")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "El paquete es obligatorio.")]
        public int IdPaquete { get; set; }

        [Required(ErrorMessage = "La cantidad de noches es obligatoria.")]
        [Range(1, 365, ErrorMessage = "La cantidad de noches debe ser mayor a 0.")]
        public int CantidadNoches { get; set; }

        [Required(ErrorMessage = "La cantidad de personas es obligatoria.")]
        [Range(1, 100, ErrorMessage = "La cantidad de personas debe ser mayor a 0.")]
        public int CantidadPersonas { get; set; }

        [Required(ErrorMessage = "El método de pago es obligatorio.")]
        [StringLength(20)]
        public string? MetodoPago { get; set; }

        [StringLength(50)]
        public string? NumeroCheque { get; set; }

        [StringLength(100)]
        public string? BancoCheque { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Descuento { get; set; }

        public decimal IVA { get; set; }

        public decimal TotalFinal { get; set; }

        public decimal Prima { get; set; }

        public decimal Mensualidad { get; set; }

        public decimal TipoCambio { get; set; }

        public decimal TotalDolares { get; set; }

        public DateTime FechaReservacion { get; set; }

        [ForeignKey("Cedula")]
        [JsonIgnore]
        public Cliente? Cliente { get; set; }

        [ForeignKey("IdPaquete")]
        [JsonIgnore]
        public Paquete? Paquete { get; set; }
    }
}
