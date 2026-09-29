namespace BienestarApi.Services;

/// <summary>Error de negocio de la encuesta SUS (ya respondida, falta la encuesta final, etc.).</summary>
public class EncuestaSusException(string message) : Exception(message);
