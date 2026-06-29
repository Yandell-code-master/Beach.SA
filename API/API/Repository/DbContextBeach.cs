using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Repository
{
    public class DbContextBeach : DbContext
    {

        public DbContextBeach(DbContextOptions<DbContextBeach> options) : base(options)
        {

        }

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Paquete> Paquetes { get; set; }

        public DbSet<Reservacion> Reservaciones { get; set; }

        public DbSet<Factura> Facturas { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Factura>(entity =>
            {
                entity.ToTable("Facturas");

                entity.HasKey(e => e.IdFactura);

                entity.Property(e => e.PrecioPorNoche).HasColumnType("decimal(18,2)");
                entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Descuento).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PorcentajeDescuento).HasColumnType("decimal(18,2)");
                entity.Property(e => e.MontoGravable).HasColumnType("decimal(18,2)");
                entity.Property(e => e.IVA).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalFinal).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TipoCambio).HasColumnType("decimal(18,4)");
                entity.Property(e => e.TotalDolares).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.Reservacion)
                      .WithMany()
                      .HasForeignKey(e => e.IdReservacion)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.IdUsuario);
                entity.HasIndex(e => e.Email).IsUnique();
            });
        }
    }
}
