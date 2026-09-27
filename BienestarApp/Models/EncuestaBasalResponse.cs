namespace BienestarApp.Models;

/// <summary>Encuesta guardada, tal como la devuelve BienestarApi.</summary>
public class EncuestaBasalResponse
{
    public int Id { get; set; }
    public FaseEncuesta Fase { get; set; }
    public int PuntajePHQ9 { get; set; }
    public int PuntajeSISCO { get; set; }
    public int PuntajeIPAQ { get; set; }
    public DateTime Fecha { get; set; }
    public bool RequiereAtencionInmediata { get; set; }
}
