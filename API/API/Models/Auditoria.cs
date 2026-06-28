using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Auditoria
    {
        [Key]
        public int IdAuditoria { get; set; }

        [StringLength(100)] public string? Usuario { get; set; }
        [StringLength(50)] public string? Accion { get; set; }   // LOGIN, CREATE, UPDATE, DELETE
        [StringLength(100)] public string? Entidad { get; set; }
        [StringLength(500)] public string? Detalle { get; set; }
        [StringLength(50)] public string? Ip { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}