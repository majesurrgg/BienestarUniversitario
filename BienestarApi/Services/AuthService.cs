using BienestarApi.Data;
using BienestarApi.DTOs.Auth;
using BienestarApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BienestarApi.Services;

/// <summary>
/// Orquesta registro/login: valida reglas de negocio, guarda en la base con
/// el DbContext, y delega en <see cref="ITokenService"/> la emisión del JWT.
/// El Controller no toca el DbContext directamente ni sabe cómo se hashea
/// una contraseña — solo llama a este servicio.
/// </summary>
public class AuthService(
    BienestarDbContext db,
    ITokenService tokenService,
    IPasswordHasher<Cuenta> passwordHasher) : IAuthService
{
    public async Task<AuthResponse> RegistrarAsync(RegisterRequest request)
    {
        var emailNormalizado = request.Email.Trim().ToLowerInvariant();

        if (await db.Cuentas.AnyAsync(c => c.Email == emailNormalizado))
            throw new AuthException("Ya existe una cuenta registrada con ese email.");

        if (await db.Usuarios.AnyAsync(u => u.CodigoUniversitario == request.CodigoUniversitario))
            throw new AuthException("Ya existe un usuario registrado con ese código universitario.");

        var usuario = new Usuario
        {
            Nombre = request.Nombre.Trim(),
            CodigoUniversitario = request.CodigoUniversitario.Trim(),
            Carrera = request.Carrera.Trim(),
            FechaRegistro = DateTime.UtcNow,
        };

        var cuenta = new Cuenta
        {
            Usuario = usuario,
            Email = emailNormalizado,
            FechaCreacion = DateTime.UtcNow,
        };
        // PasswordHasher necesita la instancia para el hash, pero no lee ni
        // escribe nada de "Cuenta" aparte de PasswordHash: solo la usa como
        // parámetro de tipo genérico (podría ser cualquier clase).
        cuenta.PasswordHash = passwordHasher.HashPassword(cuenta, request.Password);

        db.Usuarios.Add(usuario);
        db.Cuentas.Add(cuenta);
        await db.SaveChangesAsync();

        var (token, expira) = tokenService.GenerarToken(usuario, cuenta);
        return new AuthResponse
        {
            Token = token,
            ExpiraUtc = expira,
            UsuarioId = usuario.Id,
            Nombre = usuario.Nombre,
            Email = cuenta.Email,
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var emailNormalizado = request.Email.Trim().ToLowerInvariant();

        var cuenta = await db.Cuentas
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(c => c.Email == emailNormalizado);

        // Mensaje idéntico si el email no existe o si la contraseña es
        // incorrecta: no le decimos a quien intenta entrar cuál de las dos
        // falló, para no revelar qué emails están registrados.
        if (cuenta is null)
            throw new AuthException("Email o contraseña incorrectos.");

        var resultado = passwordHasher.VerifyHashedPassword(cuenta, cuenta.PasswordHash, request.Password);
        if (resultado == PasswordVerificationResult.Failed)
            throw new AuthException("Email o contraseña incorrectos.");

        var (token, expira) = tokenService.GenerarToken(cuenta.Usuario, cuenta);
        return new AuthResponse
        {
            Token = token,
            ExpiraUtc = expira,
            UsuarioId = cuenta.Usuario.Id,
            Nombre = cuenta.Usuario.Nombre,
            Email = cuenta.Email,
        };
    }
}
