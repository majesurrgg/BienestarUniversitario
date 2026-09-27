using CommunityToolkit.Mvvm.ComponentModel;

namespace BienestarApp.ViewModels.Items;

/// <summary>
/// Una pregunta del PHQ-9 en pantalla. Respuesta arranca en -1 (nada
/// seleccionado) para poder distinguir "no contestó" de "contestó 0 (ningún
/// día)" al validar el formulario — son cosas distintas.
/// </summary>
public partial class PreguntaPhq9Item : ObservableObject
{
    public string Texto { get; }
    public int Numero { get; }

    [ObservableProperty]
    private int respuesta = -1;

    public PreguntaPhq9Item(int numero, string texto)
    {
        Numero = numero;
        Texto = texto;
    }
}
