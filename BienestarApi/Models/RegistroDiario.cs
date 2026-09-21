namespace BienestarApi.Models;

/// <summary>
/// Tabla de HECHOS: un registro diario de bienestar autorreportado por el
/// estudiante (check-in). Es la tabla que más crece (hasta 30 usuarios x 28
/// días ≈ 840 filas en el piloto) y la que se conecta con las dos
/// dimensiones (Usuario, Tiempo), como corresponde en un esquema estrella.
/// </summary>
public class RegistroDiario
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public int TiempoId { get; set; }
    public Tiempo Tiempo { get; set; } = null!;

    /// <summary>Nivel de estrés autopercibido, escala Likert 1 (muy bajo) a 10 (muy alto).</summary>
    public int NivelEstres { get; set; }

    /// <summary>Calidad de sueño autopercibida, escala Likert 1 (muy mala) a 5 (muy buena).</summary>
    public int CalidadSueno { get; set; }

    /// <summary>Minutos de actividad física reportados ese día.</summary>
    public int MinutosActividadFisica { get; set; }

    /// <summary>Estado de ánimo autopercibido, escala Likert 1 (muy malo) a 5 (muy bueno).</summary>
    public int EstadoAnimo { get; set; }

    /// <summary>Marca de tiempo de creación del registro (auditoría; distinta de la fecha "de negocio" en Tiempo).</summary>
    public DateTime FechaCreacion { get; set; }
}
