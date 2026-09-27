namespace BienestarApi.Services;

/// <summary>
/// Error de negocio del check-in diario (por ejemplo, ya registró hoy). Igual
/// que <see cref="AuthException"/>: el controlador la traduce a un 4xx con
/// mensaje legible en vez de un 500.
/// </summary>
public class RegistroDiarioException(string message) : Exception(message);
