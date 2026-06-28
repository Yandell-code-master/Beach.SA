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

        public DbSet<Factura> Facturas { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Funcion> Funciones { get; set; }
        public DbSet<RolFuncion> RolFunciones { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }   // omití si no usás auditoría

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
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.IdUsuario);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasOne(e => e.Rol)
                      .WithMany(r => r.Usuarios)
                      .HasForeignKey(e => e.IdRol)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(e => e.IdRol);
                entity.HasIndex(e => e.Nombre).IsUnique();
            });

            modelBuilder.Entity<Funcion>(entity =>
            {
                entity.ToTable("Funciones");
                entity.HasKey(e => e.IdFuncion);
                entity.HasIndex(e => e.Codigo).IsUnique();
            });

            modelBuilder.Entity<RolFuncion>(entity =>
            {
                entity.ToTable("RolFunciones");
                entity.HasKey(e => e.IdRolFuncion);
                entity.HasOne(e => e.Rol)
                      .WithMany(r => r.RolFunciones)
                      .HasForeignKey(e => e.IdRol)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Funcion)
                      .WithMany(f => f.RolFunciones)
                      .HasForeignKey(e => e.IdFuncion)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.IdRol, e.IdFuncion }).IsUnique();
            });

            modelBuilder.Entity<Auditoria>(entity =>   // omití si no usás auditoría
            {
                entity.ToTable("Auditoria");
                entity.HasKey(e => e.IdAuditoria);
            });
        }
    }
}
