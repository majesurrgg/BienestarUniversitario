using System.ComponentModel;
using BienestarApp.ViewModels;

namespace BienestarApp.Views;

public partial class RespiracionPage : ContentPage
{
    private const double EscalaMinima = 0.55;

    private readonly RespiracionViewModel viewModel;

    public RespiracionPage(RespiracionViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
        viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    // Al salir de la pantalla se corta el ejercicio, para que el
    // temporizador no siga corriendo en segundo plano.
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        viewModel.Detener();
    }

    // La animación del círculo es presentación pura: el ViewModel solo dice
    // en qué fase está, y aquí se traduce a crecer o achicarse.
    private async void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(RespiracionViewModel.Fase)) return;

        Circulo.CancelAnimations();
        switch (viewModel.Fase)
        {
            case FaseRespiracion.Inhalar:
                await Circulo.ScaleToAsync(1.0, 4000, Easing.SinInOut);
                break;
            case FaseRespiracion.Exhalar:
                await Circulo.ScaleToAsync(EscalaMinima, 6000, Easing.SinInOut);
                break;
            default:
                await Circulo.ScaleToAsync(EscalaMinima, 300, Easing.CubicOut);
                break;
        }
    }
}
