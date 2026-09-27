using BienestarApp.Models;

namespace BienestarApp.Services;

/// <summary>
/// Encuesta basal del estudiante. Se encarga de conseguir el token de la
/// sesión (vía <see cref="IAuthService"/>) para que el ViewModel no tenga
/// que tocarlo — mismo patrón que <see cref="IRegistroDiarioService"/>.
/// </summary>
public interface IEncuestaBasalService
{
    Task<EncuestaBasalResponse> RegistrarAsync(EncuestaBasalRequest request);

    /// <summary>Fases (Basal/Final) que el usuario ya completó.</summary>
    Task<List<FaseEncuesta>> ObtenerFasesCompletadasAsync();
}
