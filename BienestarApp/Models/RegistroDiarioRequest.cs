namespace BienestarApp.Models;

/// <summary>
/// DTO de transporte hacia BienestarApi (POST /api/registrosdiarios). No lleva
/// usuario ni fecha: la API los toma del token y del reloj del servidor.
/// </summary>
public class RegistroDiarioRequest
{
    public int NivelEstres { get; set; }
    public int CalidadSueno { get; set; }
    public int MinutosActividadFisica { get; set; }
    public int EstadoAnimo { get; set; }
}
