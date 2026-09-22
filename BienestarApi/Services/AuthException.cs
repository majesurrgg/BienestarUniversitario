namespace BienestarApi.Services;

/// <summary>
/// Error de negocio de autenticación (credenciales inválidas, email ya
/// registrado, etc.). Se distingue de una excepción "inesperada" para que
/// el controlador la traduzca a un código HTTP 400/401 en vez de un 500.
/// </summary>
public class AuthException(string message) : Exception(message);
