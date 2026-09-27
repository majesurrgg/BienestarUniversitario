using BienestarApp.Models;

namespace BienestarApp.Services;

public class EncuestaBasalService(IAuthService authService, IApiService apiService) : IEncuestaBasalService
{
    public async Task<EncuestaBasalResponse> RegistrarAsync(EncuestaBasalRequest request) =>
        await apiService.CrearEncuestaBasalAsync(await ObtenerTokenAsync(), request);

    private async Task<string> ObtenerTokenAsync() =>
        await authService.ObtenerTokenAsync() ?? throw new SesionExpiradaException();
}
