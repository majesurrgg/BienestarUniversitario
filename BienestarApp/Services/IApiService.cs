using BienestarApp.Models;

namespace BienestarApp.Services;

/// <summary>
/// Único punto de la app que habla HTTP con BienestarApi. Los ViewModels
/// no usan HttpClient directamente: llaman a esta interfaz, así se puede
/// reemplazar por una implementación falsa en pruebas.
/// </summary>
public interface IApiService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);

    /// <summary>Llama a un endpoint protegido con el token dado, solo para comprobar que la sesión es válida.</summary>
    Task<bool> VerificarSesionAsync(string token);
}
