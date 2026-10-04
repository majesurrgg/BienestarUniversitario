namespace BienestarApi.DTOs.EncuestasBasal;

/// <summary>
/// Qué encuestas del diseño pre-post puede hacer el estudiante ahora. La app
/// lo usa para mostrar u ocultar botones sin adivinar reglas por su cuenta:
/// la fecha de la encuesta final la decide el servidor (configuración
/// "Piloto:FechaHabilitaEncuestaFinal").
/// </summary>
public class EstadoEncuestasResponse
{
    public bool BasalCompletada { get; set; }
    public bool FinalCompletada { get; set; }

    /// <summary>Desde qué día se puede responder la encuesta final (null = sin restricción de fecha).</summary>
    public DateOnly? FechaHabilitaFinal { get; set; }

    /// <summary>true si hoy ya se puede responder la final: basal hecha, final pendiente y fecha alcanzada.</summary>
    public bool FinalDisponible { get; set; }

    /// <summary>true si ya respondió la evaluación de usabilidad (SUS).</summary>
    public bool SusCompletada { get; set; }

    /// <summary>true si puede responder la SUS ahora: final hecha y SUS pendiente.</summary>
    public bool SusDisponible { get; set; }
}
