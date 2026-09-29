namespace BienestarApp.Models;

/// <summary>
/// DTO de transporte hacia BienestarApi (POST /api/auth/register). Su forma
/// coincide a propósito con <c>BienestarApi.DTOs.Auth.RegisterRequest</c>,
/// pero es una clase independiente: la app no referencia el proyecto del
/// backend, solo se pone de acuerdo con su contrato JSON.
/// </summary>
public class RegisterRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string CodigoUniversitario { get; set; } = string.Empty;
    public string Carrera { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? CodigoInvitacion { get; set; }
    public bool AceptaConsentimiento { get; set; }
    public string? VersionConsentimiento { get; set; }
}
