using BienestarApi.DTOs.Auth;

namespace BienestarApi.Services;

public interface IAuthService
{
    /// <summary>Crea Usuario + Cuenta y devuelve un token, listo para usar sin loguearse de nuevo.</summary>
    Task<AuthResponse> RegistrarAsync(RegisterRequest request);

    /// <summary>Valida credenciales contra la Cuenta y devuelve un token si son correctas.</summary>
    Task<AuthResponse> LoginAsync(LoginRequest request);
}
