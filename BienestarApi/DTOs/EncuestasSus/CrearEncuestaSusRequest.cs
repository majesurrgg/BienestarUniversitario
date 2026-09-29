using System.ComponentModel.DataAnnotations;

namespace BienestarApi.DTOs.EncuestasSus;

/// <summary>
/// Respuestas del System Usability Scale. Igual que en el PHQ-9, se envían
/// las respuestas ítem por ítem y el puntaje lo calcula el servidor: nunca
/// se confía en un total calculado por la app.
/// </summary>
public class CrearEncuestaSusRequest
{
    /// <summary>Las 10 respuestas en orden (1 = totalmente en desacuerdo ... 5 = totalmente de acuerdo).</summary>
    [Required, MinLength(10), MaxLength(10)]
    public List<int> Respuestas { get; set; } = [];

    [MaxLength(1000)]
    public string? ComentarioLoMasUtil { get; set; }

    [MaxLength(1000)]
    public string? ComentarioMejoras { get; set; }
}
