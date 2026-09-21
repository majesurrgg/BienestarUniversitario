using BienestarApp.ViewModels;

namespace BienestarApp.Views;

public partial class MainPage : ContentPage
{
    // El ViewModel se inyecta por constructor (ver MauiProgram.cs).
    // La Vista no crea su propio ViewModel ni contiene lógica de negocio;
    // solo se encarga de layout/binding.
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
