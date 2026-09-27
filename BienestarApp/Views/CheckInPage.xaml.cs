using BienestarApp.ViewModels;

namespace BienestarApp.Views;

public partial class CheckInPage : ContentPage
{
    private readonly CheckInViewModel viewModel;

    public CheckInPage(CheckInViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
    }

    // Igual que MainPage: el ciclo de vida de la página se reenvía al
    // ViewModel, que decide si mostrar el formulario o el resumen de hoy.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = viewModel.InicializarAsync();
    }
}
