using BienestarApp.Models;

namespace BienestarApp.Services;

/// <summary>
/// Check-in diario del estudiante. Se encarga de conseguir el token de la
/// sesión (vía <see cref="IAuthService"/>) para que los ViewModels no
/// tengan que tocarlo.
/// </summary>
public interface IRegistroDiarioService
{
    Task<RegistroDiarioResponse> RegistrarHoyAsync(RegistroDiarioRequest request);

    /// <summary>Check-in de hoy, o null si todavía no lo hizo.</summary>
    Task<RegistroDiarioResponse?> ObtenerDeHoyAsync();

    /// <summary>Historial de los últimos <paramref name="dias"/> días, para "Mi progreso".</summary>
    Task<List<RegistroDiarioResponse>> ObtenerHistorialAsync(int dias = 7);
}
