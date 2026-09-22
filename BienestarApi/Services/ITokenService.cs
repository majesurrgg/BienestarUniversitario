using BienestarApi.Models;

namespace BienestarApi.Services;

public interface ITokenService
{
    /// <summary>Genera un JWT firmado para la cuenta indicada.</summary>
    (string Token, DateTime ExpiraUtc) GenerarToken(Usuario usuario, Cuenta cuenta);
}
