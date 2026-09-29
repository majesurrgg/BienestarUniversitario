using BienestarApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BienestarApi.Data;

/// <summary>
/// DbContext único para el Data Warehouse dimensional simplificado del
/// piloto. Usa Code First: el modelo se define en C# (clases en Models/)
/// y EF Core genera el esquema de SQL Server a partir de él mediante
/// migraciones (ver "dotnet ef migrations add InicialDW").
/// </summary>
public class BienestarDbContext : DbContext
{
    public BienestarDbContext(DbContextOptions<BienestarDbContext> options)
        : base(options)
    {
    }

    // Dimensiones
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Cuenta> Cuentas => Set<Cuenta>();
    public DbSet<Tiempo> Tiempos => Set<Tiempo>();

    // Hechos
    public DbSet<RegistroDiario> RegistrosDiarios => Set<RegistroDiario>();
    public DbSet<EncuestaBasal> EncuestasBasal => Set<EncuestaBasal>();
    public DbSet<EncuestaSUS> EncuestasSUS => Set<EncuestaSUS>();
    public DbSet<AlertaRiesgo> AlertasRiesgo => Set<AlertaRiesgo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---------- Dimensión Usuario ----------
        modelBuilder.Entity<Usuario>(entity =>
        {
            // Nombre de tabla explícito con prefijo "Dim" para que el
            // esquema en SQL Server sea legible como estrella (Dim*/Fact-like).
            entity.ToTable("DimUsuario");
            entity.HasIndex(u => u.CodigoUniversitario).IsUnique();
        });

        // ---------- Cuenta (1:1 con Usuario) ----------
        modelBuilder.Entity<Cuenta>(entity =>
        {
            entity.HasIndex(c => c.Email).IsUnique();
            entity.HasIndex(c => c.UsuarioId).IsUnique(); // fuerza la relación 1:1

            entity.HasOne(c => c.Usuario)
                  .WithOne(u => u.Cuenta)
                  .HasForeignKey<Cuenta>(c => c.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ---------- Dimensión Tiempo ----------
        modelBuilder.Entity<Tiempo>(entity =>
        {
            entity.ToTable("DimTiempo");
            entity.HasIndex(t => t.Fecha).IsUnique(); // una sola fila por fecha calendario
        });

        // ---------- Hecho: RegistroDiario ----------
        modelBuilder.Entity<RegistroDiario>(entity =>
        {
            entity.HasOne(r => r.Usuario)
                  .WithMany(u => u.RegistrosDiarios)
                  .HasForeignKey(r => r.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Restrict (no cascade) hacia la dimensión Tiempo: SQL Server no
            // permite dos rutas de cascada convergiendo en la misma tabla
            // (Usuario->RegistroDiario y Tiempo->RegistroDiario), y además
            // conceptualmente una dimensión de tiempo no debería "arrastrar"
            // el borrado de hechos.
            entity.HasOne(r => r.Tiempo)
                  .WithMany(t => t.RegistrosDiarios)
                  .HasForeignKey(r => r.TiempoId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Un estudiante no debería tener dos check-ins el mismo día.
            entity.HasIndex(r => new { r.UsuarioId, r.TiempoId }).IsUnique();
        });

        // ---------- Hecho: EncuestaBasal ----------
        modelBuilder.Entity<EncuestaBasal>(entity =>
        {
            entity.HasOne(e => e.Usuario)
                  .WithMany(u => u.EncuestasBasal)
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Como mucho una encuesta basal por usuario y fase (Basal/Final).
            entity.HasIndex(e => new { e.UsuarioId, e.Fase }).IsUnique();
        });

        // ---------- Hecho: EncuestaSUS ----------
        modelBuilder.Entity<EncuestaSUS>(entity =>
        {
            entity.Property(e => e.PuntajeSUS).HasColumnType("decimal(5,2)");
            entity.Property(e => e.RespuestasSUS).HasMaxLength(40);
            entity.Property(e => e.ComentarioLoMasUtil).HasMaxLength(1000);
            entity.Property(e => e.ComentarioMejoras).HasMaxLength(1000);

            // La SUS se aplica una sola vez por estudiante, al cierre.
            entity.HasIndex(e => e.UsuarioId).IsUnique();

            entity.HasOne(e => e.Usuario)
                  .WithMany(u => u.EncuestasSUS)
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ---------- Hecho: AlertaRiesgo ----------
        modelBuilder.Entity<AlertaRiesgo>(entity =>
        {
            entity.HasOne(a => a.Usuario)
                  .WithMany(u => u.AlertasRiesgo)
                  .HasForeignKey(a => a.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(a => a.EncuestaBasal)
                  .WithMany()
                  .HasForeignKey(a => a.EncuestaBasalId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
