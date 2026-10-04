using BienestarApi.DTOs.EncuestasSus;

namespace BienestarApi.Services;

public interface IEncuestaSusService
{
    /// <summary>Guarda la SUS del usuario (una sola vez, después de la encuesta final).</summary>
    Task<EncuestaSusResponse> CrearAsync(int usuarioId, CrearEncuestaSusRequest request);
}
