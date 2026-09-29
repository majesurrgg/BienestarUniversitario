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

    // Check-in diario (Sprint 3). Lanzan SesionExpiradaException si la API responde 401.
    Task<RegistroDiarioResponse> CrearRegistroDiarioAsync(string token, RegistroDiarioRequest request);

    /// <summary>Check-in de hoy, o null si todavía no lo hizo.</summary>
    Task<RegistroDiarioResponse?> ObtenerRegistroDiarioDeHoyAsync(string token);

    /// <summary>Historial de check-ins de los últimos <paramref name="dias"/> días, más reciente primero.</summary>
    Task<List<RegistroDiarioResponse>> ObtenerHistorialRegistroDiarioAsync(string token, int dias);

    /// <summary>Encuesta basal (Sprint 3). Lanza SesionExpiradaException si la API responde 401.</summary>
    Task<EncuestaBasalResponse> CrearEncuestaBasalAsync(string token, EncuestaBasalRequest request);

    /// <summary>Fases (Basal/Final) que el usuario ya completó.</summary>
    Task<List<FaseEncuesta>> ObtenerFasesEncuestaBasalCompletadasAsync(string token);

    /// <summary>Encuestas hechas y disponibles (basal, final con su fecha, SUS).</summary>
    Task<EstadoEncuestasResponse> ObtenerEstadoEncuestasAsync(string token);

    /// <summary>Evaluación de usabilidad (SUS). Lanza SesionExpiradaException si la API responde 401.</summary>
    Task<EncuestaSusResponse> CrearEncuestaSusAsync(string token, EncuestaSusRequest request);
}
