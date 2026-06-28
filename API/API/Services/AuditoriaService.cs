using API.Models;
using API.Repository;

namespace API.Services
{
    public class AuditoriaService : IAuditoriaService
    {
        private readonly DbContextBeach _dbContextBeach;

        public AuditoriaService(DbContextBeach dbContextBeach)
        {
            _dbContextBeach = dbContextBeach;
        }

        public void Registrar(string? usuario, string? accion, string? entidad, string? detalle, string? ip)
        {
            _dbContextBeach.Auditorias.Add(new Auditoria
            {
                Usuario = string.IsNullOrEmpty(usuario) ? "Anónimo" : usuario,
                Accion = accion,
                Entidad = entidad,
                Detalle = detalle,
                Ip = ip,
                Fecha = DateTime.Now
            });
            _dbContextBeach.SaveChanges();
        }
    }
}