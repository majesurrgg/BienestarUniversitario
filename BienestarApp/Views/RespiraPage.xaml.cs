namespace BienestarApp.Views;

/// <summary>
/// Pausa de respiración guiada (4-7-8 simplificado): no guarda nada, no
/// llama a la API, es puramente una herramienta de bienestar para el
/// estudiante — algo que la app le da a cambio de pedirle datos todos los
/// días. Se puede abrir desde el check-in en cualquier momento.
/// </summary>
public partial class RespiraPage : ContentPage
{
    private CancellationTokenSource? cts;

    public RespiraPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        cts = new CancellationTokenSource();
        _ = CicloDeRespiracionAsync(cts.Token);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        cts?.Cancel();
        Circulo.CancelAnimations();
    }

    private async Task CicloDeRespiracionAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                InstruccionLabel.Text = "Inhala...";
                await Circulo.ScaleToAsync(1.3, 4000, Easing.SinInOut);
                if (token.IsCancellationRequested) break;

                InstruccionLabel.Text = "Sostén...";
                await Task.Delay(1000, token);

                InstruccionLabel.Text = "Exhala...";
                await Circulo.ScaleToAsync(0.75, 5000, Easing.SinInOut);
            }
        }
        catch (TaskCanceledException)
        {
            // Se cerró la pantalla a mitad del ciclo — nada que hacer.
        }
    }

    private async void OnTerminarClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync("..");
}
