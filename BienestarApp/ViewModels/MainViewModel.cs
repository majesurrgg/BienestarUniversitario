namespace BienestarApp.ViewModels;

/// <summary>
/// ViewModel de la página principal. En el Sprint 1 no contiene lógica de
/// negocio: solo demuestra el enlace (binding) entre Vista y ViewModel.
/// En Sprint 2 aquí se orquestarán llamadas a Services/ (por ejemplo,
/// autenticación y consumo de la API) para poblar el estado de la pantalla.
/// </summary>
public partial class MainViewModel : BaseViewModel
{
    public MainViewModel()
    {
        Title = "Bienestar Universitario";
    }
}
