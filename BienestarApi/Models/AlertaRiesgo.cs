namespace BienestarApi.Models;

/// <summary>
/// Tabla de HECHOS que registra la ejecución del protocolo ético para el
/// ítem 9 del PHQ-9 (ideación suicida/autolesión). Cuando ese ítem supera
/// el umbral definido, el sistema debe mostrar un mensaje de ayuda/derivación
/// (Sprint 2/3); esta tabla deja constancia auditable de que esa protección
/// se activó, con qué encuesta la disparó y si el mensaje llegó a mostrarse.
/// </summary>
public class AlertaRiesgo
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    /// <summary>Encuesta basal (PHQ-9) que originó la alerta, si aplica. Permite trazabilidad para el comité de ética.</summary>
    public int? EncuestaBasalId { get; set; }
    public EncuestaBasal? EncuestaBasal { get; set; }

    public DateTime Fecha { get; set; }

    /// <summary>true si se le mostró efectivamente al estudiante el mensaje de ayuda/derivación.</summary>
    public bool SeMostroMensajeAyuda { get; set; }
}
