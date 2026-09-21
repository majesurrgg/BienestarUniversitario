using Microsoft.AspNetCore.Mvc;

namespace BienestarApi.Controllers;

/// <summary>
/// Controlador mínimo solo para verificar que el esqueleto (routing,
/// DI, pipeline de middlewares) funciona de punta a punta en el Sprint 1.
/// No es lógica de negocio: no toca el DbContext ni ninguna entidad.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "ok", timestampUtc = DateTime.UtcNow });
}
