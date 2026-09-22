using BienestarApp.Models;

namespace BienestarApp.Services;

public class AuthService(IApiService apiService) : IAuthService
{
    // Claves de SecureStorage: guarda en el llavero cifrado del sistema
    // operativo (Keystore en Android), no en preferencias de texto plano.
    private const string ClaveToken = "auth_token";
    private const string ClaveNombre = "auth_nombre";
    private const string ClaveEmail = "auth_email";

    public async Task<AuthResponse> RegistrarAsync(RegisterRequest request)
    {
        var respuesta = await apiService.RegisterAsync(request);
        await GuardarSesionAsync(respuesta);
        return respuesta;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var respuesta = await apiService.LoginAsync(request);
        await GuardarSesionAsync(respuesta);
        return respuesta;
    }

    public Task CerrarSesionAsync()
    {
        SecureStorage.Default.Remove(ClaveToken);
        SecureStorage.Default.Remove(ClaveNombre);
        SecureStorage.Default.Remove(ClaveEmail);
        return Task.CompletedTask;
    }

    public async Task<bool> HaySesionActivaAsync() =>
        !string.IsNullOrEmpty(await ObtenerTokenAsync());

    public Task<string?> ObtenerNombreAsync() => SecureStorage.Default.GetAsync(ClaveNombre);

    public Task<string?> ObtenerTokenAsync() => SecureStorage.Default.GetAsync(ClaveToken);

    private static async Task GuardarSesionAsync(AuthResponse respuesta)
    {
        await SecureStorage.Default.SetAsync(ClaveToken, respuesta.Token);
        await SecureStorage.Default.SetAsync(ClaveNombre, respuesta.Nombre);
        await SecureStorage.Default.SetAsync(ClaveEmail, respuesta.Email);
    }
}
