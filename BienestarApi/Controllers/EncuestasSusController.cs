using System.Security.Claims;
using BienestarApi.DTOs.EncuestasSus;
using BienestarApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BienestarApi.Controllers;

/// <summary>
/// Evaluación de usabilidad de la app (SUS). Igual que los demás
/// controladores de datos: exige JWT y el usuario sale del token.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EncuestasSusController(IEncuestaSusService encuestaSusService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<EncuestaSusResponse>> Crear(CrearEncuestaSusRequest request)
    {
        try
        {
            return Ok(await encuestaSusService.CrearAsync(UsuarioIdDelToken(), request));
        }
        catch (EncuestaSusException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    private int UsuarioIdDelToken() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
