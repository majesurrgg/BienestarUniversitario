namespace BienestarApi.DTOs.RegistrosDiarios;

/// <summary>Check-in tal como se devuelve a la app (sin datos internos como UsuarioId o TiempoId).</summary>
public class RegistroDiarioResponse
{
    public int Id { get; set; }
    public DateOnly Fecha { get; set; }
    public int NivelEstres { get; set; }
    public int CalidadSueno { get; set; }
    public int MinutosActividadFisica { get; set; }
    public int EstadoAnimo { get; set; }
}
