using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BienestarApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace BienestarApi.Services;

/// <summary>
/// Firma los JWT con la misma clave/issuer/audience que <c>Program.cs</c>
/// configuró para VALIDARLOS. Si algún día cambia esa configuración, solo
/// hay que tocarla en <c>appsettings.json</c> — este servicio la lee de ahí.
/// </summary>
public class TokenService(IConfiguration configuration) : ITokenService
{
    public (string Token, DateTime ExpiraUtc) GenerarToken(Usuario usuario, Cuenta cuenta)
    {
        var jwtSection = configuration.GetSection("Jwt");
        var key = jwtSection["Key"]
            ?? throw new InvalidOperationException("Falta configurar Jwt:Key en appsettings.json");
        var expiryMinutes = jwtSection.GetValue<int?>("ExpiryMinutes") ?? 60;
        var expiraUtc = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, cuenta.Email),
            new Claim(ClaimTypes.Name, usuario.Nombre),
        };

        var credenciales = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: expiraUtc,
            signingCredentials: credenciales);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiraUtc);
    }
}
