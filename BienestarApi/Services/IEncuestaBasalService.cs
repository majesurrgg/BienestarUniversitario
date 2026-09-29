using BienestarApi.DTOs.EncuestasBasal;
using BienestarApi.Models.Enums;

namespace BienestarApi.Services;

public interface IEncuestaBasalService
{
    /// <summary>Guarda la encuesta basal del usuario para la fase indicada. Falla si ya existe una para esa fase.</summary>
    Task<EncuestaBasalResponse> CrearAsync(int usuarioId, CrearEncuestaBasalRequest request);

    /// <summary>Fases (Basal/Final) que el usuario ya completó — para que la app no deje reenviar una fase ya hecha.</summary>
    Task<List<FaseEncuesta>> ObtenerFasesCompletadasAsync(int usuarioId);

    /// <summary>Fases completadas + desde cuándo se habilita la final (ver <see cref="EstadoEncuestasResponse"/>).</summary>
    Task<EstadoEncuestasResponse> ObtenerEstadoAsync(int usuarioId);
}
