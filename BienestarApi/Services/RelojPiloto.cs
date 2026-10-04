namespace BienestarApi.Services;

/// <summary>
/// "Hoy" según la zona horaria del estudio (Perú), no la del servidor ni
/// UTC: la API corre en Azure (West US 2), y a las 9 p. m. en Lima ya es el
/// día siguiente en UTC — el check-in quedaría en la fecha equivocada.
/// </summary>
public class RelojPiloto(IConfiguration configuration) : IRelojPiloto
{
    private readonly TimeZoneInfo zona =
        TimeZoneInfo.FindSystemTimeZoneById(configuration["ZonaHoraria"] ?? "America/Lima");

    public DateOnly Hoy() => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zona));
}
