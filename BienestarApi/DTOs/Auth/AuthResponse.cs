namespace BienestarApi.DTOs.Auth;

/// <summary>
/// Lo que recibe la app tras un registro/login correcto: el JWT y algunos
/// datos básicos para mostrar sin tener que pedirlos aparte.
/// </summary>
public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraUtc { get; set; }
    public int UsuarioId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
