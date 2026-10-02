using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BienestarApp.ViewModels;

/// <summary>Fase actual del ejercicio; la Vista anima el círculo según esta fase.</summary>
public enum FaseRespiracion
{
    EnReposo,
    Inhalar,
    Exhalar,
}

/// <summary>
/// "Respira 1 minuto": respiración guiada de 60 s (6 ciclos de inhalar 4 s
/// y exhalar 6 s; la exhalación más larga es lo que ayuda a calmarse). Es
/// un ejercicio de pausa, no una intervención clínica, y no guarda nada en
/// la base de datos.
/// </summary>
public partial class RespiracionViewModel : BaseViewModel
{
    private const int DuracionTotal = 60;
    private const int SegundosInhalar = 4;
    private const int SegundosExhalar = 6;

    private CancellationTokenSource? cancelacion;

    [ObservableProperty]
    private FaseRespiracion fase = FaseRespiracion.EnReposo;

    [ObservableProperty]
    private string indicacion = "Busca una posición cómoda y toca «Empezar».";

    [ObservableProperty]
    private int segundosRestantes = DuracionTotal;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Detenido))]
    private bool enCurso;

    public bool Detenido => !EnCurso;

    public RespiracionViewModel()
    {
        Title = "Respira 1 minuto";
    }

    [RelayCommand]
    private async Task EmpezarAsync()
    {
        if (EnCurso) return;

        cancelacion = new CancellationTokenSource();
        EnCurso = true;
        SegundosRestantes = DuracionTotal;

        try
        {
            while (SegundosRestantes > 0)
            {
                Fase = FaseRespiracion.Inhalar;
                Indicacion = "Inhala lento por la nariz…";
                await EsperarAsync(SegundosInhalar, cancelacion.Token);

                Fase = FaseRespiracion.Exhalar;
                Indicacion = "Exhala despacio por la boca…";
                await EsperarAsync(SegundosExhalar, cancelacion.Token);
            }
            Indicacion = "¡Muy bien! Tómate un momento antes de seguir. 💚";
        }
        catch (OperationCanceledException)
        {
            Indicacion = "Puedes volver a intentarlo cuando quieras.";
        }
        finally
        {
            Fase = FaseRespiracion.EnReposo;
            EnCurso = false;
            SegundosRestantes = DuracionTotal;
        }
    }

    /// <summary>También lo llama la Vista al salir de la pantalla, para no dejar el temporizador corriendo.</summary>
    [RelayCommand]
    public void Detener() => cancelacion?.Cancel();

    private async Task EsperarAsync(int segundos, CancellationToken token)
    {
        for (var i = 0; i < segundos && SegundosRestantes > 0; i++)
        {
            await Task.Delay(1000, token);
            SegundosRestantes--;
        }
    }
}
