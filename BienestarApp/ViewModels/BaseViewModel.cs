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
}
