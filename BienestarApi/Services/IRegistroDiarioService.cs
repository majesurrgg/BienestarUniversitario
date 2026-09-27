using BienestarApi.DTOs.RegistrosDiarios;

namespace BienestarApi.Services;

public interface IRegistroDiarioService
{
    /// <summary>Guarda el check-in de hoy del usuario. Falla si ya existe uno para hoy.</summary>
    Task<RegistroDiarioResponse> CrearAsync(int usuarioId, CrearRegistroDiarioRequest request);

    /// <summary>Devuelve el check-in de hoy del usuario, o null si todavía no lo hizo.</summary>
    Task<RegistroDiarioResponse?> ObtenerDeHoyAsync(int usuarioId);

    /// <summary>Últimos check-ins del usuario, más reciente primero (para la pantalla "Mi progreso").</summary>
    Task<List<RegistroDiarioResponse>> ObtenerHistorialAsync(int usuarioId, int dias);
}
