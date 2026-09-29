namespace BienestarApp.Models;

/// <summary>
/// Copia en el celular del estado del estudiante (encuestas hechas e
/// historial de check-ins). Existe para AHORRAR consultas a la base de
/// Azure: el nivel gratuito cobra por el tiempo que la base pasa despierta,
/// y cada consulta aislada la despierta. Con esta copia, la app consulta
/// la API como mucho una vez al día al abrirse, y el resto del día trabaja
/// con lo que ya sabe (y lo actualiza sola al guardar algo nuevo).
/// </summary>
public class DatosLocales
{
    /// <summary>Último día en que se trajo el estado completo desde la API (null = nunca).</summary>
    public DateOnly? UltimaSincronizacion { get; set; }

    public bool BasalCompletada { get; set; }
    public bool FinalCompletada { get; set; }
    public DateOnly? FechaHabilitaFinal { get; set; }
    public bool SusCompletada { get; set; }

    /// <summary>Check-ins recientes, del más nuevo al más antiguo.</summary>
    public List<RegistroDiarioResponse> Historial { get; set; } = [];

    public RegistroDiarioResponse? CheckInDe(DateOnly fecha) => Historial.FirstOrDefault(r => r.Fecha == fecha);

    // Las mismas reglas que EstadoEncuestasResponse de la API, recalculadas
    // aquí para no tener que preguntar cada día si ya llegó la fecha.
    public bool FinalDisponible(DateOnly hoy) =>
        BasalCompletada && !FinalCompletada && (FechaHabilitaFinal is null || hoy >= FechaHabilitaFinal);

    public bool SusDisponible => FinalCompletada && !SusCompletada;
}
