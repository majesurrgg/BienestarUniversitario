namespace BienestarApi.Services;

/// <summary>
/// Error de negocio de la encuesta basal (ej. fase ya registrada, PHQ-9 mal
/// formado). Igual que <see cref="RegistroDiarioException"/>: el controlador
/// la traduce a un 4xx con mensaje legible en vez de un 500.
/// </summary>
public class EncuestaBasalException(string message) : Exception(message);
