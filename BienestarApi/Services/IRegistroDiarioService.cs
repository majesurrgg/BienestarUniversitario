using BienestarApi.DTOs.RegistrosDiarios;

namespace BienestarApi.Services;

public interface IRegistroDiarioService
{
    /// <summary>Guarda el check-in de hoy del usuario. Falla si ya existe uno para hoy.</summary>
    Task<RegistroDiarioResponse> CrearAsync(int usuarioId, CrearRegistroDiarioRequest request);

    /// <summary>Devuelve el check-in de hoy del usuario, o null si todavía no lo hizo.</summary>
    Task<RegistroDiarioResponse?> ObtenerDeHoyAsync(int usuarioId);
}
