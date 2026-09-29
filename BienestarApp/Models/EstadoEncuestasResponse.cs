namespace BienestarApp.Models;

/// <summary>Qué encuestas puede hacer el estudiante ahora (GET /api/encuestasbasal/estado).</summary>
public class EstadoEncuestasResponse
{
    public bool BasalCompletada { get; set; }
    public bool FinalCompletada { get; set; }
    public DateOnly? FechaHabilitaFinal { get; set; }
    public bool FinalDisponible { get; set; }
    public bool SusCompletada { get; set; }
    public bool SusDisponible { get; set; }
}
