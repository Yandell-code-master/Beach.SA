using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class Factura
    {
        [Key]
        public int IdFactura { get; set; }

        [Required]
        public int IdReservacion { get; set; }

        // --- Snapshot del cliente ---
        [Required]
        [StringLength(20)]
        public string Cedula { get; set; } = null!;

        [StringLength(100)]
        public string? NombreCompleto { get; set; }

        [StringLength(100)]
        public string? CorreoElectronico { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }

        // --- Snapshot del paquete ---
        [StringLength(100)]
        public string? NombrePaquete { get; set; }

        public decimal PrecioPorNoche { get; set; }

        // --- Snapshot de la reservación ---
        public int CantidadNoches { get; set; }

        public int CantidadPersonas { get; set; }

        // --- Desglose financiero ---
        public decimal SubTotal { get; set; }

        public decimal Descuento { get; set; }

        public decimal PorcentajeDescuento { get; set; }

        public decimal MontoGravable { get; set; }

        public decimal IVA { get; set; }

        public decimal TotalFinal { get; set; }

        public decimal TipoCambio { get; set; }

        public decimal TotalDolares { get; set; }

        [StringLength(20)]
        public string? MetodoPago { get; set; }

        public DateTime FechaEmision { get; set; }

        // Relación con Reservacion
        [ForeignKey("IdReservacion")]
        [JsonIgnore]
        public Reservacion? Reservacion { get; set; }
    }
}
