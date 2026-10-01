using System.ComponentModel;
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
        viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    // Igual que MainPage: el ciclo de vida de la página se reenvía al
    // ViewModel, que decide si mostrar el formulario o el resumen de hoy.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = viewModel.InicializarAsync();
    }

    // Pequeña celebración al guardar el día: el corazón "late" dos veces y
    // el celular vibra suave. Es pura presentación (como el rebote de la
    // encuesta basal), por eso vive aquí y no en el ViewModel.
    private async void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(CheckInViewModel.RecienGuardado) || !viewModel.RecienGuardado) return;

        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch
        {
            // Celular sin vibración o sin permiso: la animación basta.
        }

        for (var i = 0; i < 2; i++)
        {
            await CorazonCelebracion.ScaleToAsync(1.35, 140, Easing.CubicOut);
            await CorazonCelebracion.ScaleToAsync(1.0, 140, Easing.CubicIn);
        }
    }
}
