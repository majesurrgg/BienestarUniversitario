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
}
