using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BienestarApp.Models;

namespace BienestarApp.Services;

public class ApiService(HttpClient http) : IApiService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request) =>
        await PostAsync<RegisterRequest, AuthResponse>("api/auth/register", request);

    public async Task<AuthResponse> LoginAsync(LoginRequest request) =>
        await PostAsync<LoginRequest, AuthResponse>("api/auth/login", request);

    public async Task<bool> VerificarSesionAsync(string token)
    {
        using var mensaje = new HttpRequestMessage(HttpMethod.Get, "api/health/secure");
        mensaje.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await http.SendAsync(mensaje);
        return response.IsSuccessStatusCode;
    }

    public async Task<RegistroDiarioResponse> CrearRegistroDiarioAsync(string token, RegistroDiarioRequest request)
    {
        using var mensaje = new HttpRequestMessage(HttpMethod.Post, "api/registrosdiarios")
        {
            Content = JsonContent.Create(request),
        };
        mensaje.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await http.SendAsync(mensaje);
        await LanzarSiEsErrorAsync(response);
        return (await response.Content.ReadFromJsonAsync<RegistroDiarioResponse>())!;
    }

    public async Task<RegistroDiarioResponse?> ObtenerRegistroDiarioDeHoyAsync(string token)
    {
        using var mensaje = new HttpRequestMessage(HttpMethod.Get, "api/registrosdiarios/hoy");
        mensaje.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await http.SendAsync(mensaje);
        await LanzarSiEsErrorAsync(response);

        // 204 = todavía no hizo el check-in de hoy.
        if (response.StatusCode == HttpStatusCode.NoContent)
            return null;

        return await response.Content.ReadFromJsonAsync<RegistroDiarioResponse>();
    }

    public async Task<List<RegistroDiarioResponse>> ObtenerHistorialRegistroDiarioAsync(string token, int dias)
    {
        using var mensaje = new HttpRequestMessage(HttpMethod.Get, $"api/registrosdiarios/historial?dias={dias}");
        mensaje.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await http.SendAsync(mensaje);
        await LanzarSiEsErrorAsync(response);
        return (await response.Content.ReadFromJsonAsync<List<RegistroDiarioResponse>>())!;
    }

    public async Task<EncuestaBasalResponse> CrearEncuestaBasalAsync(string token, EncuestaBasalRequest request)
    {
        using var mensaje = new HttpRequestMessage(HttpMethod.Post, "api/encuestasbasal")
        {
            Content = JsonContent.Create(request),
        };
        mensaje.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await http.SendAsync(mensaje);
        await LanzarSiEsErrorAsync(response);
        return (await response.Content.ReadFromJsonAsync<EncuestaBasalResponse>())!;
    }

    public async Task<List<FaseEncuesta>> ObtenerFasesEncuestaBasalCompletadasAsync(string token)
    {
        using var mensaje = new HttpRequestMessage(HttpMethod.Get, "api/encuestasbasal/fases-completadas");
        mensaje.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await http.SendAsync(mensaje);
        await LanzarSiEsErrorAsync(response);
        return (await response.Content.ReadFromJsonAsync<List<FaseEncuesta>>())!;
    }

    private async Task<TResponse> PostAsync<TRequest, TResponse>(string ruta, TRequest body)
    {
        using var response = await http.PostAsJsonAsync(ruta, body);
        await LanzarSiEsErrorAsync(response);
        return (await response.Content.ReadFromJsonAsync<TResponse>())!;
    }

    private static async Task LanzarSiEsErrorAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        // Los endpoints protegidos responden 401 sin cuerpo cuando el token
        // venció; el login responde 401 CON mensaje ("Email o contraseña
        // incorrectos"). Por eso solo se trata como sesión expirada si no
        // hay un mensaje de la API.
        var error = await LeerErrorAsync(response);
        if (response.StatusCode == HttpStatusCode.Unauthorized && error is null)
            throw new SesionExpiradaException();

        throw new ApiException(error ?? "No se pudo completar la solicitud.");
    }

    private static async Task<string?> LeerErrorAsync(HttpResponseMessage response)
    {
        try
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            return string.IsNullOrWhiteSpace(error?.Message) ? null : error.Message;
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            return null; // cuerpo vacío o sin la forma { "message": ... }
        }
    }
}
