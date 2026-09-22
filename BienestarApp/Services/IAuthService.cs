using BienestarApp.Models;

namespace BienestarApp.Services;

/// <summary>
/// Guarda y expone la sesión del estudiante. Es el único lugar de la app
/// que toca el token JWT (vía SecureStorage) — los ViewModels le preguntan
/// a este servicio, nunca leen el token directamente.
/// </summary>
public interface IAuthService
{
    Task<AuthResponse> RegistrarAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task CerrarSesionAsync();

    Task<bool> HaySesionActivaAsync();
    Task<string?> ObtenerNombreAsync();
    Task<string?> ObtenerTokenAsync();
}
