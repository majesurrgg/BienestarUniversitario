using System.Net.Http.Headers;
using System.Net.Http.Json;
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

    private async Task<TResponse> PostAsync<TRequest, TResponse>(string ruta, TRequest body)
    {
        using var response = await http.PostAsJsonAsync(ruta, body);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new ApiException(error?.Message ?? "No se pudo completar la solicitud.");
        }

        return (await response.Content.ReadFromJsonAsync<TResponse>())!;
    }
}
