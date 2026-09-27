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

    // Ancho de la barra en pixeles (16px por punto de estrés, escala 1-10 =
    // hasta 160px) — evita necesitar una librería de gráficos para algo tan
    // simple como una barra horizontal.
    public double AnchoBarraEstres => NivelEstres * 16.0;

    public Color ColorBarraEstres => NivelEstres switch
    {
        <= 3 => Color.FromArgb("#2C5D3B"), // verde: estrés bajo
        <= 6 => Color.FromArgb("#9C6B22"), // ámbar: estrés medio
        _ => Color.FromArgb("#B3261E"),    // rojo: estrés alto
    };
}
