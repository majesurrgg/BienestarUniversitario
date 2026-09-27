using System.ComponentModel.DataAnnotations;
using BienestarApi.Models.Enums;

namespace BienestarApi.DTOs.EncuestasBasal;

/// <summary>
/// Encuesta basal que envía la app (fase Basal o Final del diseño pre-post).
/// No trae UsuarioId a propósito: sale del JWT, igual que en el check-in
/// diario (ver <see cref="Controllers.RegistrosDiariosController"/>).
/// </summary>
public class CrearEncuestaBasalRequest
{
    [Required]
    public FaseEncuesta Fase { get; set; }

    /// <summary>
    /// Las 9 respuestas del PHQ-9, en orden (posición 0 = ítem 1 ...
    /// posición 8 = ítem 9), cada una 0-3. El servidor calcula el total y
    /// revisa el ítem 9 — nunca confía en un total que mande el cliente,
    /// para que no se pueda evadir el protocolo de riesgo enviando un total
    /// bajo con el ítem 9 en positivo.
    /// </summary>
    [Required, MinLength(9), MaxLength(9)]
    public List<int> RespuestasPhq9 { get; set; } = [];

    /// <summary>
    /// TODO (pendiente de confirmar con el asesor/comité de ética): esto
    /// debería calcularse a partir de las respuestas ítem por ítem del
    /// inventario SISCO, igual que PHQ-9. Por ahora se recibe el puntaje ya
    /// calculado porque no se ha confirmado qué versión del instrumento
    /// (SISCO original 2007 vs. SISCO II 2018) usa la tesis.
    /// </summary>
    [Range(0, 100, ErrorMessage = "El puntaje SISCO debe estar entre 0 y 100.")]
    public int PuntajeSISCO { get; set; }

    /// <summary>Puntaje IPAQ (MET-minutos/semana), calculado en la app a partir del IPAQ corto (7 ítems).</summary>
    [Range(0, int.MaxValue, ErrorMessage = "El puntaje IPAQ no puede ser negativo.")]
    public int PuntajeIPAQ { get; set; }
}
