using BienestarApp.Models;

namespace BienestarApp.Services;

public class RegistroDiarioService(IAuthService authService, IApiService apiService) : IRegistroDiarioService
{
    public async Task<RegistroDiarioResponse> RegistrarHoyAsync(RegistroDiarioRequest request) =>
        await apiService.CrearRegistroDiarioAsync(await ObtenerTokenAsync(), request);

    public async Task<RegistroDiarioResponse?> ObtenerDeHoyAsync() =>
        await apiService.ObtenerRegistroDiarioDeHoyAsync(await ObtenerTokenAsync());

    private async Task<string> ObtenerTokenAsync() =>
        await authService.ObtenerTokenAsync() ?? throw new SesionExpiradaException();
}
