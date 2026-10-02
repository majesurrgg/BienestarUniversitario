namespace BienestarApp.Services;

/// <summary>Compartido entre MainViewModel (vista previa) y ProgresoViewModel (detalle), para no repetir la lógica.</summary>
public static class CalculadorRacha
{
    /// <summary>
    /// Días seguidos con check-in, contando hacia atrás desde hoy o ayer (si
    /// todavía no hizo el de hoy, no corta la racha). <paramref
    /// name="fechasDesc"/> debe venir ordenada de más reciente a más antigua.
    /// </summary>
    public static int Calcular(IReadOnlyList<DateOnly> fechasDesc)
    {
        if (fechasDesc.Count == 0) return 0;

        var hoy = DateOnly.FromDateTime(DateTime.Now);
        var esperado = fechasDesc[0];
        if (esperado != hoy && esperado != hoy.AddDays(-1)) return 0;

        var racha = 0;
        foreach (var fecha in fechasDesc)
        {
            if (fecha != esperado) break;
            racha++;
            esperado = esperado.AddDays(-1);
        }
        return racha;
    }

    /// <summary>
    /// La racha más larga de todo el historial (no solo la actual): los
    /// logros ganados no se pierden si después se corta la racha.
    /// </summary>
    public static int Mejor(IEnumerable<DateOnly> fechas)
    {
        var ordenadas = fechas.Distinct().Order().ToList();
        var mejor = 0;
        var actual = 0;
        for (var i = 0; i < ordenadas.Count; i++)
        {
            actual = i > 0 && ordenadas[i] == ordenadas[i - 1].AddDays(1) ? actual + 1 : 1;
            mejor = Math.Max(mejor, actual);
        }
        return mejor;
    }
}
