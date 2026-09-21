namespace BienestarApi.Models;

/// <summary>
/// Dimensión "Tiempo" del modelo estrella. Una fila por cada día calendario
/// relevante para el estudio (se puede pre-poblar de una sola vez para las
/// 4 semanas del piloto). Tenerla separada, en vez de guardar solo un
/// DateTime en cada hecho, permite agregar por semana/día de la semana sin
/// recalcular esas partes en cada consulta (idea estándar de un Data
/// Warehouse dimensional en esquema estrella).
/// </summary>
public class Tiempo
{
    public int Id { get; set; }

    public DateOnly Fecha { get; set; }

    /// <summary>Año calendario (necesario para no mezclar semana 1 de 2026 con semana 1 de 2027, etc.).</summary>
    public int Anio { get; set; }

    /// <summary>Número de semana ISO-8601 dentro del año.</summary>
    public int Semana { get; set; }

    /// <summary>Día de la semana (Lunes, Martes, ...).</summary>
    public DayOfWeek DiaSemana { get; set; }

    // --- Navegación ---
    public ICollection<RegistroDiario> RegistrosDiarios { get; set; } = new List<RegistroDiario>();
}
