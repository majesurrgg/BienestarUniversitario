using BienestarApp.ViewModels;

namespace BienestarApp.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel viewModel;

    // El ViewModel se inyecta por constructor (ver MauiProgram.cs).
    // La Vista no crea su propio ViewModel ni contiene lógica de negocio;
    // solo se encarga de layout/binding.
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
    }

    // Único código-behind con "lógica": el evento de ciclo de vida de la
    // página (OnAppearing) no existe en el ViewModel, así que se reenvía
    // aquí. Todo lo que decide hacer al aparecer vive en InicializarAsync().
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = viewModel.InicializarAsync();
    }
}
