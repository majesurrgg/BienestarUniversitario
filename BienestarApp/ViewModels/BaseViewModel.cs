using CommunityToolkit.Mvvm.ComponentModel;

namespace BienestarApp.ViewModels;

/// <summary>
/// ViewModel base del que heredan todos los ViewModels de la app.
/// ObservableObject (CommunityToolkit.Mvvm) genera automáticamente
/// INotifyPropertyChanged para las propiedades marcadas con [ObservableProperty],
/// evitando escribir a mano el boilerplate de notificación de cambios.
/// </summary>
public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string title = string.Empty;

    // "Ojito" de las pantallas con contraseña (login y registro): alterna
    // entre ocultarla y mostrarla para que el estudiante revise lo que
    // escribió. Vive aquí para no repetirlo en cada ViewModel.
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OcultarPassword))]
    [NotifyPropertyChangedFor(nameof(IconoOjo))]
    private bool mostrarPassword;

    public bool OcultarPassword => !MostrarPassword;

    public string IconoOjo => MostrarPassword ? "🙈" : "👁";

    [CommunityToolkit.Mvvm.Input.RelayCommand]
    private void AlternarPassword() => MostrarPassword = !MostrarPassword;
}
