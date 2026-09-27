namespace BienestarApp.Models;

/// <summary>
/// DTO de transporte hacia BienestarApi (POST /api/encuestasbasal). No lleva
/// usuario: la API lo toma del token.
/// </summary>
public class EncuestaBasalRequest
{
    public FaseEncuesta Fase { get; set; }
    public List<int> RespuestasPhq9 { get; set; } = [];
    public int PuntajeSISCO { get; set; }
    public int PuntajeIPAQ { get; set; }
}
