namespace BienestarApp.Services;

/// <summary>
/// La API respondió 401: el token venció (dura 60 minutos) o no es válido.
/// Los ViewModels la atrapan para cerrar la sesión y volver al login.
/// </summary>
public class SesionExpiradaException() : Exception("Tu sesión expiró. Vuelve a iniciar sesión.");
