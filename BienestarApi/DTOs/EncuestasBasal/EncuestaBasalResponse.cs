using BienestarApi.Models.Enums;

namespace BienestarApi.DTOs.EncuestasBasal;

/// <summary>Encuesta guardada, tal como se devuelve a la app (sin UsuarioId ni otros datos internos).</summary>
public class EncuestaBasalResponse
{
    public int Id { get; set; }
    public FaseEncuesta Fase { get; set; }
    public int PuntajePHQ9 { get; set; }
    public int PuntajeSISCO { get; set; }
    public int PuntajeIPAQ { get; set; }
    public DateTime Fecha { get; set; }

    /// <summary>true si el ítem 9 dio positivo: la app debe mostrar de inmediato el mensaje de ayuda/derivación.</summary>
    public bool RequiereAtencionInmediata { get; set; }
}
