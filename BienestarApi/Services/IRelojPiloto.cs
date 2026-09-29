namespace BienestarApi.Services;

/// <summary>
/// "Qué día es hoy" para el estudio, en la zona horaria del piloto (Perú).
/// Compartido por el check-in diario y la encuesta final para que ambos usen
/// exactamente la misma regla de fecha.
/// </summary>
public interface IRelojPiloto
{
    DateOnly Hoy();
}
