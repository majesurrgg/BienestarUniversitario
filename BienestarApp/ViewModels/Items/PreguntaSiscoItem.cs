using CommunityToolkit.Mvvm.ComponentModel;

namespace BienestarApp.ViewModels.Items;

/// <summary>
/// Un ítem del SISCO SV-21 (dimensiones Estresores/Síntomas/Estrategias).
/// Escala oficial de 6 valores: Nunca(0), Casi nunca(1), Rara vez(2),
/// Algunas veces(3), Casi siempre(4), Siempre(5) — Barraza (2018), "Ficha
/// técnica del Inventario SISCO SV-21". Respuesta arranca en -1 (nada
/// seleccionado), igual que <see cref="PreguntaPhq9Item"/>.
/// </summary>
public partial class PreguntaSiscoItem : ObservableObject
{
    public string Texto { get; }

    [ObservableProperty]
    private int respuesta = -1;

    public PreguntaSiscoItem(string texto)
    {
        Texto = texto;
    }
}
