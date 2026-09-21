namespace BienestarApi.Models;

/// <summary>
/// Tabla de HECHOS: resultado del System Usability Scale, aplicado al final
/// del piloto para evaluar la usabilidad de la app (no el bienestar del
/// estudiante). Se modela aparte de <see cref="EncuestaBasal"/> porque mide
/// algo distinto (la herramienta, no la persona) y solo se aplica una vez.
/// </summary>
public class EncuestaSUS
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    /// <summary>Puntaje SUS, 0-100 (promedio ponderado de las 10 preguntas del instrumento).</summary>
    public decimal PuntajeSUS { get; set; }

    public DateTime FechaAplicacion { get; set; }
}
