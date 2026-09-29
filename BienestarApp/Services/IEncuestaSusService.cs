using BienestarApp.Models;

namespace BienestarApp.Services;

/// <summary>Evaluación de usabilidad (SUS). Consigue el token igual que los demás servicios.</summary>
public interface IEncuestaSusService
{
    Task<EncuestaSusResponse> RegistrarAsync(EncuestaSusRequest request);
}
