namespace BienestarApp.Models;

/// <summary>DTO hacia POST /api/encuestassus. El puntaje lo calcula la API.</summary>
public class EncuestaSusRequest
{
    /// <summary>Las 10 respuestas en orden, cada una de 1 a 5.</summary>
    public List<int> Respuestas { get; set; } = [];
    public string? ComentarioLoMasUtil { get; set; }
    public string? ComentarioMejoras { get; set; }
}
