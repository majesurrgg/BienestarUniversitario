using BienestarApi.DTOs.EncuestasBasal;

namespace BienestarApi.Services;

public interface IEncuestaBasalService
{
    /// <summary>Guarda la encuesta basal del usuario para la fase indicada. Falla si ya existe una para esa fase.</summary>
    Task<EncuestaBasalResponse> CrearAsync(int usuarioId, CrearEncuestaBasalRequest request);
}
