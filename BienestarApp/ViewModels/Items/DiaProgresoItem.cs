using Microsoft.Maui.Graphics;

namespace BienestarApp.ViewModels.Items;

/// <summary>Un día del historial en la pantalla "Mi progreso". Datos ya cerrados (no cambian), por eso no es ObservableObject.</summary>
public class DiaProgresoItem
{
    private static readonly string[] DiasSemana = ["dom", "lun", "mar", "mié", "jue", "vie", "sáb"];

    public required DateOnly Fecha { get; init; }
    public required int NivelEstres { get; init; }
    public required int CalidadSueno { get; init; }
    public required int EstadoAnimo { get; init; }
    public required int MinutosActividadFisica { get; init; }

    public string FechaTexto => $"{DiasSemana[(int)Fecha.DayOfWeek]} {Fecha:dd/MM}";

    public string EstresTexto => $"{NivelEstres}/10";

    public string DetalleTexto =>
        $"Sueño {CalidadSueno}/5  ·  Ánimo {EstadoAnimo}/5  ·  Actividad {MinutosActividadFisica} min";

    /// <summary>
    /// Parte llena de la barra, proporcional (0 a 1) sobre la escala 1-10.
    /// Se usa con AbsoluteLayout en modo proporcional, así la barra ocupa
    /// siempre el mismo ancho total en cualquier tamaño de pantalla.
    /// </summary>
    public Rect LimitesBarraEstres => new(0, 0, Math.Clamp(NivelEstres / 10.0, 0.05, 1), 1);

    public Color ColorBarraEstres => NivelEstres switch
    {
        <= 3 => Color.FromArgb("#3E9C7A"), // verde: estrés bajo
        <= 6 => Color.FromArgb("#E0A33B"), // ámbar: estrés medio
        _ => Color.FromArgb("#D9534F"),    // rojo: estrés alto
    };
}
