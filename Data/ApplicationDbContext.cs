using Microsoft.EntityFrameworkCore;
using ControlVehiculos.Models.Entities;

namespace ControlVehiculos.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Rol> Roles { get; set; }
    public DbSet<EstadoTurno> EstadosTurno { get; set; }
    public DbSet<EstadoVehiculo> EstadosVehiculo { get; set; }
    public DbSet<ResultadoEvaluacion> ResultadosEvaluacion { get; set; }
    public DbSet<Chequeo> Chequeos { get; set; }
    public DbSet<Propietario> Propietarios { get; set; }
    public DbSet<Vehiculo> Vehiculos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<UsuarioRol> UsuarioRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<CentroInspeccion> Centros { get; set; }
    public DbSet<Turno> Turnos { get; set; }
    public DbSet<Evaluacion> Evaluaciones { get; set; }
    public DbSet<EvaluacionDetalle> EvaluacionDetalles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Rol
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Codigo).IsUnique();
        });

        // EstadoTurno
        modelBuilder.Entity<EstadoTurno>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Codigo).IsUnique();
        });

        // EstadoVehiculo
        modelBuilder.Entity<EstadoVehiculo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Codigo).IsUnique();
        });

        // ResultadoEvaluacion
        modelBuilder.Entity<ResultadoEvaluacion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Codigo).IsUnique();
        });

        // Chequeo
        modelBuilder.Entity<Chequeo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
        });

        // Propietario
        modelBuilder.Entity<Propietario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Telefono).HasMaxLength(50);
        });

        // Vehiculo
        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Matricula).IsRequired().HasMaxLength(12);
            entity.Property(e => e.Marca).HasMaxLength(100);
            entity.Property(e => e.Modelo).HasMaxLength(100);
            entity.HasIndex(e => e.Matricula).IsUnique();

            entity.HasOne(e => e.Propietario)
                .WithMany(p => p.Vehiculos)
                .HasForeignKey(e => e.PropietarioId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.EstadoVehiculo)
                .WithMany(ev => ev.Vehiculos)
                .HasForeignKey(e => e.EstadoVehiculoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // UsuarioRol (Many-to-Many)
        modelBuilder.Entity<UsuarioRol>(entity =>
        {
            entity.HasKey(e => new { e.UsuarioId, e.RolId });

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.UsuarioRoles)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Rol)
                .WithMany(r => r.UsuarioRoles)
                .HasForeignKey(e => e.RolId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RefreshToken
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(500);

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CentroInspeccion
        modelBuilder.Entity<CentroInspeccion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Direccion).HasMaxLength(500);
        });

        // Turno
        modelBuilder.Entity<Turno>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MotivoCancelacion).HasMaxLength(200);

            entity.HasOne(e => e.Vehiculo)
                .WithMany(v => v.Turnos)
                .HasForeignKey(e => e.VehiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Centro)
                .WithMany(c => c.Turnos)
                .HasForeignKey(e => e.CentroId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.EstadoTurno)
                .WithMany(et => et.Turnos)
                .HasForeignKey(e => e.EstadoTurnoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint for (CentroId, FechaHora)
            entity.HasIndex(e => new { e.CentroId, e.FechaHora }).IsUnique();
        });

        // Evaluacion
        modelBuilder.Entity<Evaluacion>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Turno)
                .WithOne(t => t.Evaluacion)
                .HasForeignKey<Evaluacion>(e => e.TurnoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Inspector)
                .WithMany(u => u.Evaluaciones)
                .HasForeignKey(e => e.InspectorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Resultado)
                .WithMany(r => r.Evaluaciones)
                .HasForeignKey(e => e.ResultadoEvaluacionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint: one evaluation per turno
            entity.HasIndex(e => e.TurnoId).IsUnique();
        });

        // EvaluacionDetalle
        modelBuilder.Entity<EvaluacionDetalle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Observacion).HasMaxLength(500);

            entity.HasOne(e => e.Evaluacion)
                .WithMany(ev => ev.Detalles)
                .HasForeignKey(e => e.EvaluacionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Chequeo)
                .WithMany(c => c.EvaluacionDetalles)
                .HasForeignKey(e => e.ChequeoId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
