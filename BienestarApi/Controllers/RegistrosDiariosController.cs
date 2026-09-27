using System.Security.Claims;
using BienestarApi.DTOs.RegistrosDiarios;
using BienestarApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BienestarApi.Controllers;

/// <summary>
/// Check-in diario del estudiante. Todo el controlador exige JWT: el
/// usuario siempre sale del token (claim NameIdentifier que emite
/// TokenService), nunca de lo que mande la app en el cuerpo o la URL.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RegistrosDiariosController(IRegistroDiarioService registroDiarioService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<RegistroDiarioResponse>> Crear(CrearRegistroDiarioRequest request)
    {
        try
        {
            return Ok(await registroDiarioService.CrearAsync(UsuarioIdDelToken(), request));
        }
        catch (RegistroDiarioException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>Check-in de hoy, o 204 (sin contenido) si todavía no lo hizo.</summary>
    [HttpGet("hoy")]
    public async Task<ActionResult<RegistroDiarioResponse>> ObtenerDeHoy()
    {
        var registro = await registroDiarioService.ObtenerDeHoyAsync(UsuarioIdDelToken());
        return registro is null ? NoContent() : Ok(registro);
    }

    private int UsuarioIdDelToken() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
