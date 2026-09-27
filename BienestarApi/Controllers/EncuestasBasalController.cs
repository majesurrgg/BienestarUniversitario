using System.Security.Claims;
using BienestarApi.DTOs.EncuestasBasal;
using BienestarApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BienestarApi.Controllers;

/// <summary>
/// Encuesta basal del estudiante (PHQ-9, SISCO, IPAQ). Todo el controlador
/// exige JWT: el usuario siempre sale del token (claim NameIdentifier que
/// emite TokenService), nunca de lo que mande la app en el cuerpo o la URL.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EncuestasBasalController(IEncuestaBasalService encuestaBasalService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<EncuestaBasalResponse>> Crear(CrearEncuestaBasalRequest request)
    {
        try
        {
            return Ok(await encuestaBasalService.CrearAsync(UsuarioIdDelToken(), request));
        }
        catch (EncuestaBasalException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    private int UsuarioIdDelToken() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
