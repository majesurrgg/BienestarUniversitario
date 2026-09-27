using System.ComponentModel;
using BienestarApp.ViewModels;

namespace BienestarApp.Views;

public partial class EncuestaBasalPage : ContentPage
{
    private readonly EncuestaBasalViewModel viewModel;

    public EncuestaBasalPage(EncuestaBasalViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
        viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = viewModel.InicializarAsync();
    }

    // El "toque vivo" que se pidió para la encuesta: un pequeño rebote del
    // ícono cada vez que se avanza/retrocede de paso. Es animación de
    // presentación pura (no lógica de negocio), por eso vive en el
    // code-behind y no en el ViewModel — el ViewModel no debería saber que
    // existe un control visual llamado "IconoTextoLabel".
    private async void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(EncuestaBasalViewModel.Paso)) return;

        await IconoTextoLabel.ScaleToAsync(1.3, 120, Easing.CubicOut);
        await IconoTextoLabel.ScaleToAsync(1.0, 120, Easing.CubicIn);
    }
}
