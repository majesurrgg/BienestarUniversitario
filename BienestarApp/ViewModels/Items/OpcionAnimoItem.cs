using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BienestarApp.ViewModels.Items;

/// <summary>
/// Una carita del selector de ánimo en "Mi día" (escala 1-5). El comando
/// vive en el propio ítem y avisa al ViewModel con <c>alSeleccionar</c>:
/// así la plantilla del XAML no necesita "buscar" el ViewModel de la página.
/// </summary>
public partial class OpcionAnimoItem(int valor, string emoji, string etiqueta, Action<OpcionAnimoItem> alSeleccionar)
    : ObservableObject
{
    public int Valor { get; } = valor;
    public string Emoji { get; } = emoji;
    public string Etiqueta { get; } = etiqueta;

    [ObservableProperty]
    private bool seleccionado;

    [RelayCommand]
    private void Seleccionar() => alSeleccionar(this);
}
