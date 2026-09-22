using BienestarApi.DTOs.Auth;
using BienestarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BienestarApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        try
        {
            return Ok(await authService.RegistrarAsync(request));
        }
        catch (AuthException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        try
        {
            return Ok(await authService.LoginAsync(request));
        }
        catch (AuthException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
