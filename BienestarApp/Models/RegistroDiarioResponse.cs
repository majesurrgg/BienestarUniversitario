namespace BienestarApp.Models;

/// <summary>Check-in guardado, tal como lo devuelve BienestarApi.</summary>
public class RegistroDiarioResponse
{
    public int Id { get; set; }
    public DateOnly Fecha { get; set; }
    public int NivelEstres { get; set; }
    public int CalidadSueno { get; set; }
    public int MinutosActividadFisica { get; set; }
    public int EstadoAnimo { get; set; }
}
