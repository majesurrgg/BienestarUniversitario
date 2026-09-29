using System.Globalization;
using BienestarApp.Models;

namespace BienestarApp.Services;

public class AuthService(IApiService apiService, IDatosLocalesService datosLocales) : IAuthService
{
    // Claves de SecureStorage: guarda en el llavero cifrado del sistema
    // operativo (Keystore en Android), no en preferencias de texto plano.
    private const string ClaveToken = "auth_token";
    private const string ClaveNombre = "auth_nombre";
    private const string ClaveEmail = "auth_email";
    private const string ClaveExpira = "auth_expira";

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
        SecureStorage.Default.Remove(ClaveExpira);
        datosLocales.Borrar();
        return Task.CompletedTask;
    }

    /// <summary>
    /// true si hay un token guardado que todavía no venció. Se revisa en el
    /// propio celular (la API manda la fecha de vencimiento al iniciar
    /// sesión) en vez de preguntarle a la API: así abrir la app no despierta
    /// el servidor solo para confirmar la sesión.
    /// </summary>
    public async Task<bool> HaySesionActivaAsync()
    {
        if (string.IsNullOrEmpty(await ObtenerTokenAsync()))
            return false;

        var expiraTexto = await SecureStorage.Default.GetAsync(ClaveExpira);
        if (!DateTime.TryParse(expiraTexto, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var expiraUtc))
            return true; // sesión de una versión anterior sin fecha guardada: decide la API (401 si venció).

        return expiraUtc > DateTime.UtcNow.AddMinutes(1);
    }

    public Task<string?> ObtenerNombreAsync() => SecureStorage.Default.GetAsync(ClaveNombre);

    public Task<string?> ObtenerTokenAsync() => SecureStorage.Default.GetAsync(ClaveToken);

    private async Task GuardarSesionAsync(AuthResponse respuesta)
    {
        // Cuenta nueva o distinta en este celular: se descarta lo que se
        // sabía de la anterior antes de guardar la sesión.
        datosLocales.Borrar();
        await SecureStorage.Default.SetAsync(ClaveToken, respuesta.Token);
        await SecureStorage.Default.SetAsync(ClaveNombre, respuesta.Nombre);
        await SecureStorage.Default.SetAsync(ClaveEmail, respuesta.Email);
        await SecureStorage.Default.SetAsync(ClaveExpira, respuesta.ExpiraUtc.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
    }
}
