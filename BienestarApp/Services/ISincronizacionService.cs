using BienestarApp.Models;

namespace BienestarApp.Services;

/// <summary>
/// Punto único por el que las pantallas conocen el estado del estudiante
/// (encuestas e historial). Decide CUÁNDO vale la pena consultar la API:
/// como mucho una vez al día, o cuando la pantalla lo necesita sí o sí.
/// </summary>
public interface ISincronizacionService
{
    /// <summary>Lo que ya se sabe, sin consultar la API.</summary>
    DatosLocales Actual { get; }

    /// <summary>
    /// Devuelve los datos locales; solo consulta la API (estado de encuestas
    /// + historial) si todavía no se sincronizó hoy o si <paramref name="forzar"/>.
    /// </summary>
    Task<DatosLocales> ObtenerAsync(bool forzar = false);

    /// <summary>Consulta solo el estado de las encuestas (una llamada) y lo guarda.</summary>
    Task<DatosLocales> ActualizarEstadoEncuestasAsync();

    // Actualizan la copia local tras guardar algo, sin volver a consultar la API.
    void RegistrarCheckIn(RegistroDiarioResponse registro);
    void RegistrarEncuesta(FaseEncuesta fase);
    void RegistrarSus();
}
