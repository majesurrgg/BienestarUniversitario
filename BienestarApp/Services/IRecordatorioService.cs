namespace BienestarApp.Services;

/// <summary>
/// Recordatorio local del check-in diario (Sprint 4). No llama a la API en
/// el momento de sonar — usa el último estado conocido para decidir si
/// programar o cancelar, y el sistema operativo lo dispara aunque la app
/// esté cerrada.
/// </summary>
public interface IRecordatorioService
{
    /// <summary>Programa el recordatorio de hoy si <paramref name="yaHizoCheckInHoy"/> es false; lo cancela si es true.</summary>
    Task ProgramarSiFaltaAsync(bool yaHizoCheckInHoy);

    void CancelarDeHoy();
}
