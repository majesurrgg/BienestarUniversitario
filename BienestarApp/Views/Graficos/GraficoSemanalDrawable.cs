using BienestarApp.ViewModels.Items;

namespace BienestarApp.Views.Graficos;

/// <summary>
/// Gráfico de líneas de los últimos 7 días en "Mi progreso": estrés (escala
/// 1-10) y ánimo (escala 1-5), ambos llevados a 0-100 % para compararlos en
/// el mismo eje. Se dibuja a mano con GraphicsView (sin librerías externas);
/// los días sin check-in quedan como huecos en la línea.
/// </summary>
public class GraficoSemanalDrawable : IDrawable
{
    private static readonly string[] DiasSemana = ["dom", "lun", "mar", "mié", "jue", "vie", "sáb"];

    public static readonly Color ColorEstres = Color.FromArgb("#E991AE");
    public static readonly Color ColorAnimoClaro = Color.FromArgb("#1E6E6A");
    public static readonly Color ColorAnimoOscuro = Color.FromArgb("#5FC2BC");

    public IReadOnlyList<DiaProgresoItem> Dias { get; set; } = [];

    public void Draw(ICanvas canvas, RectF area)
    {
        var oscuro = Application.Current?.RequestedTheme == AppTheme.Dark;
        var colorTexto = oscuro ? Color.FromArgb("#ACACAC") : Color.FromArgb("#6E6E6E");
        var colorGuia = oscuro ? Color.FromArgb("#404040") : Color.FromArgb("#E1E1E1");
        var colorAnimo = oscuro ? ColorAnimoOscuro : ColorAnimoClaro;

        const float margenInferior = 22;
        const float margenX = 16;
        var alto = area.Height - margenInferior - 8;
        var ancho = area.Width - 2 * margenX;
        var paso = ancho / 6f;

        // Líneas guía: 0 %, 50 %, 100 %.
        canvas.StrokeColor = colorGuia;
        canvas.StrokeSize = 1;
        foreach (var fraccion in new[] { 0f, 0.5f, 1f })
        {
            var y = 8 + alto * (1 - fraccion);
            canvas.DrawLine(margenX, y, area.Width - margenX, y);
        }

        var hoy = DateOnly.FromDateTime(DateTime.Now);
        var porFecha = Dias.ToDictionary(d => d.Fecha);
        var puntosEstres = new List<PointF?>();
        var puntosAnimo = new List<PointF?>();

        canvas.FontColor = colorTexto;
        canvas.FontSize = 11;
        for (var i = 0; i < 7; i++)
        {
            var fecha = hoy.AddDays(i - 6);
            var x = margenX + paso * i;
            canvas.DrawString(DiasSemana[(int)fecha.DayOfWeek], x - 20, area.Height - margenInferior + 4, 40, 16,
                HorizontalAlignment.Center, VerticalAlignment.Top);

            if (porFecha.TryGetValue(fecha, out var dia))
            {
                puntosEstres.Add(new PointF(x, 8 + alto * (1 - dia.NivelEstres / 10f)));
                puntosAnimo.Add(new PointF(x, 8 + alto * (1 - dia.EstadoAnimo / 5f)));
            }
            else
            {
                puntosEstres.Add(null);
                puntosAnimo.Add(null);
            }
        }

        DibujarLinea(canvas, puntosEstres, ColorEstres);
        DibujarLinea(canvas, puntosAnimo, colorAnimo);
    }

    private static void DibujarLinea(ICanvas canvas, List<PointF?> puntos, Color color)
    {
        canvas.StrokeColor = color;
        canvas.StrokeSize = 3;
        canvas.StrokeLineCap = LineCap.Round;
        for (var i = 1; i < puntos.Count; i++)
        {
            if (puntos[i - 1] is { } a && puntos[i] is { } b)
                canvas.DrawLine(a, b);
        }

        canvas.FillColor = color;
        foreach (var punto in puntos)
        {
            if (punto is { } p)
                canvas.FillCircle(p, 5);
        }
    }
}
