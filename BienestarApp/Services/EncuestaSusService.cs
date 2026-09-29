using BienestarApp.Models;

namespace BienestarApp.Services;

public class EncuestaSusService(IAuthService authService, IApiService apiService) : IEncuestaSusService
{
    public async Task<EncuestaSusResponse> RegistrarAsync(EncuestaSusRequest request) =>
        await apiService.CrearEncuestaSusAsync(
            await authService.ObtenerTokenAsync() ?? throw new SesionExpiradaException(), request);
}
