using System.ComponentModel.DataAnnotations;

namespace BienestarApi.DTOs.Auth;

/// <summary>
/// Datos que envía la app para registrar a un estudiante nuevo. Es un
/// contrato de transporte (DTO), separado a propósito de las entidades de
/// EF Core (<see cref="Models.Usuario"/>, <see cref="Models.Cuenta"/>):
/// así la forma de la API no cambia aunque cambie el modelo de datos.
/// </summary>
public class RegisterRequest
{
    [Required]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public string CodigoUniversitario { get; set; } = string.Empty;

    [Required]
    public string Carrera { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    public string Password { get; set; } = string.Empty;
}
