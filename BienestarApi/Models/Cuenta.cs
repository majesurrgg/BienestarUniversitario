namespace BienestarApi.Models;

/// <summary>
/// Datos de autenticación del estudiante (1:1 con <see cref="Usuario"/>).
/// El login en sí (endpoints, generación/validación de JWT) se implementa
/// en el Sprint 2; aquí solo queda modelada la forma de los datos.
/// </summary>
public class Cuenta
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hash de la contraseña (nunca se guarda en texto plano). Se calculará
    /// con un algoritmo de hashing con salt (ASP.NET Core Identity /
    /// BCrypt) al implementar el registro/login en el Sprint 2.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; }
}
