using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Repository
{
    /// <summary>
    /// Clase que maneja el context de la base de datos
    /// </summary>
    public class DbContextBeach : DbContext
    {

        public DbContextBeach(DbContextOptions<DbContextBeach> options) : base(options)
        {

        }

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Paquete> Paquetes { get; set; }

        public DbSet<Reservacion> Reservaciones { get; set; }
    }
}
