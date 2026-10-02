using BienestarApp.ViewModels.Items;

namespace BienestarApp.Services;

/// <summary>
/// Insignias por constancia, calculadas en el celular a partir del historial
/// local (sin consultar la API). Premian la constancia —que es lo que más
/// importa para la calidad de los datos del piloto— y nunca el contenido de
/// las respuestas: no hay insignia por "estrés bajo", para no empujar a
/// nadie a responder distinto de cómo se siente.
/// </summary>
public static class CalculadorLogros
{
    public static List<LogroItem> Calcular(IReadOnlyCollection<DateOnly> fechas)
    {
        var diasRegistrados = fechas.Distinct().Count();
        var mejorRacha = CalculadorRacha.Mejor(fechas);

        return
        [
            new() { Emoji = "🌱", Titulo = "Primer paso", Descripcion = "Tu primer día registrado", Desbloqueado = diasRegistrados >= 1 },
            new() { Emoji = "🔥", Titulo = "3 seguidos", Descripcion = "3 días seguidos", Desbloqueado = mejorRacha >= 3 },
            new() { Emoji = "⭐", Titulo = "Una semana", Descripcion = "7 días seguidos", Desbloqueado = mejorRacha >= 7 },
            new() { Emoji = "💪", Titulo = "Dos semanas", Descripcion = "14 días seguidos", Desbloqueado = mejorRacha >= 14 },
            new() { Emoji = "🏆", Titulo = "Constancia", Descripcion = "20 días registrados", Desbloqueado = diasRegistrados >= 20 },
        ];
    }
}
