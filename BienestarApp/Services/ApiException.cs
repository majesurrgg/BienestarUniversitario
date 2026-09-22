namespace BienestarApp.Services;

/// <summary>Error de negocio devuelto por la API (credenciales inválidas, email duplicado, etc.).</summary>
public class ApiException(string message) : Exception(message);
