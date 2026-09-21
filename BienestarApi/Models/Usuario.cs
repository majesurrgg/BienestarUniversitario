namespace BienestarApi.Models;

/// <summary>
/// Dimensión "Usuario" del modelo estrella: describe al estudiante.
/// Es una dimensión, no una tabla operacional de "usuarios de sistema";
/// los datos de autenticación viven aparte, en <see cref="Cuenta"/>
/// (separar "quién es la persona" de "cómo entra al sistema" evita mezclar
/// un dato analítico —que interesa para los reportes de la tesis— con un
/// dato de seguridad —que no debería aparecer en consultas analíticas—).
/// </summary>
public class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    /// <summary>Código único que identifica al estudiante en la universidad.</summary>
    public string CodigoUniversitario { get; set; } = string.Empty;

    public string Carrera { get; set; } = string.Empty;

    /// <summary>Fecha en la que el estudiante se registró/enroló en el estudio.</summary>
    public DateTime FechaRegistro { get; set; }

    // --- Navegación (relaciones 1:1 / 1:N) ---
    public Cuenta? Cuenta { get; set; }
    public ICollection<RegistroDiario> RegistrosDiarios { get; set; } = new List<RegistroDiario>();
    public ICollection<EncuestaBasal> EncuestasBasal { get; set; } = new List<EncuestaBasal>();
    public ICollection<EncuestaSUS> EncuestasSUS { get; set; } = new List<EncuestaSUS>();
    public ICollection<AlertaRiesgo> AlertasRiesgo { get; set; } = new List<AlertaRiesgo>();
}
