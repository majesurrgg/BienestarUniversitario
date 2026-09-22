using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BienestarApi.Controllers;

/// <summary>
/// Controlador mínimo para verificar que el esqueleto (routing, DI,
/// pipeline de middlewares) funciona de punta a punta. No es lógica de
/// negocio: no toca el DbContext ni ninguna entidad.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "ok", timestampUtc = DateTime.UtcNow });

    /// <summary>
    /// Igual que <see cref="Get"/> pero exige un JWT válido. Sirve para
    /// comprobar que el login funciona de punta a punta: si esto responde,
    /// el token que mandó la app es válido y el middleware de autenticación
    /// (configurado desde el Sprint 1) está funcionando.
    /// </summary>
    [HttpGet("secure")]
    [Authorize]
    public IActionResult GetSecure() => Ok(new
    {
        status = "ok",
        timestampUtc = DateTime.UtcNow,
        usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier),
        email = User.FindFirstValue(ClaimTypes.Email),
    });
}
