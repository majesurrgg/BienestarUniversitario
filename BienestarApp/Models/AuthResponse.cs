namespace BienestarApp.Models;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraUtc { get; set; }
    public int UsuarioId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
