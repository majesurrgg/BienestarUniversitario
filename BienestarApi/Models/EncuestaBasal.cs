using BienestarApi.Models.Enums;

namespace BienestarApi.Models;

/// <summary>
/// Tabla de HECHOS de baja frecuencia (~2 registros por estudiante: fase
/// Basal y fase Final del diseño pre-post). Agrupa los 3 instrumentos
/// estandarizados que se aplican juntos al inicio y al final del piloto.
/// </summary>
public class EncuestaBasal
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    /// <summary>PHQ-9 (Patient Health Questionnaire-9): tamizaje de depresión, rango 0-27.</summary>
    public int PuntajePHQ9 { get; set; }

    /// <summary>SISCO: inventario de estrés académico (puntaje/índice de estrés académico).</summary>
    public int PuntajeSISCO { get; set; }

    /// <summary>IPAQ (International Physical Activity Questionnaire): nivel de actividad física autorreportado (MET-min/semana).</summary>
    public int PuntajeIPAQ { get; set; }

    public DateTime Fecha { get; set; }

    /// <summary>Indica si esta aplicación corresponde a la fase Basal (pre) o Final (post) del estudio.</summary>
    public FaseEncuesta Fase { get; set; }
}
