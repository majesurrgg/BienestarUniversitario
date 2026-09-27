using System.ComponentModel.DataAnnotations;

namespace BienestarApi.DTOs.RegistrosDiarios;

/// <summary>
/// Check-in diario que envía la app. No trae UsuarioId ni fecha a propósito:
/// el usuario sale del JWT y la fecha la decide el servidor, así nadie puede
/// registrar datos a nombre de otro ni "rellenar" días pasados.
/// Los rangos coinciden con las escalas documentadas en <see cref="Models.RegistroDiario"/>.
/// </summary>
public class CrearRegistroDiarioRequest
{
    [Range(1, 10, ErrorMessage = "El nivel de estrés debe estar entre 1 y 10.")]
    public int NivelEstres { get; set; }

    [Range(1, 5, ErrorMessage = "La calidad de sueño debe estar entre 1 y 5.")]
    public int CalidadSueno { get; set; }

    [Range(0, 1440, ErrorMessage = "Los minutos de actividad física deben estar entre 0 y 1440.")]
    public int MinutosActividadFisica { get; set; }

    [Range(1, 5, ErrorMessage = "El estado de ánimo debe estar entre 1 y 5.")]
    public int EstadoAnimo { get; set; }
}
