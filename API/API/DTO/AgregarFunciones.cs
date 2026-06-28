using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class AsignarFuncionesDTO
    {
        [Required] public int IdRol { get; set; }
        [Required] public List<int> IdFunciones { get; set; } = new List<int>();
    }
}