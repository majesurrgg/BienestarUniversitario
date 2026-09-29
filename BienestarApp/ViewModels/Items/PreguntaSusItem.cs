using CommunityToolkit.Mvvm.ComponentModel;

namespace BienestarApp.ViewModels.Items;

/// <summary>
/// Una pregunta de la SUS en pantalla. <see cref="IndiceRespuesta"/> es la
/// posición elegida en el Picker (0-4, o -1 si no contestó); la respuesta
/// real del instrumento es IndiceRespuesta + 1 (escala 1-5).
/// </summary>
public partial class PreguntaSusItem : ObservableObject
{
    public int Numero { get; }
    public string Texto { get; }

    [ObservableProperty]
    private int indiceRespuesta = -1;

    public PreguntaSusItem(int numero, string texto)
    {
        Numero = numero;
        Texto = texto;
    }
}
