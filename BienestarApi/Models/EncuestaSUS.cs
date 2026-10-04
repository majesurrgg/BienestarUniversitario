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

    /// <summary>
    /// Las 10 respuestas (1-5) en orden, separadas por coma (ej. "4,2,5,1,4,2,5,1,4,2"),
    /// para poder reportar el promedio por ítem en la tesis además del total.
    /// </summary>
    public string RespuestasSUS { get; set; } = string.Empty;

    // Preguntas abiertas: la parte cualitativa del enfoque mixto de la tesis.
    public string? ComentarioLoMasUtil { get; set; }
    public string? ComentarioMejoras { get; set; }

    public DateTime FechaAplicacion { get; set; }
}
