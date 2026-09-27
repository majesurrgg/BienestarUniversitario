using BienestarApp.Models;

namespace BienestarApp.Services;

public class EncuestaBasalService(IAuthService authService, IApiService apiService) : IEncuestaBasalService
{
    public async Task<EncuestaBasalResponse> RegistrarAsync(EncuestaBasalRequest request) =>
        await apiService.CrearEncuestaBasalAsync(await ObtenerTokenAsync(), request);

    public async Task<List<FaseEncuesta>> ObtenerFasesCompletadasAsync() =>
        await apiService.ObtenerFasesEncuestaBasalCompletadasAsync(await ObtenerTokenAsync());

    private async Task<string> ObtenerTokenAsync() =>
        await authService.ObtenerTokenAsync() ?? throw new SesionExpiradaException();
}
