using System.Collections.ObjectModel;
using BienestarApp.Models;
using BienestarApp.Services;
using BienestarApp.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BienestarApp.ViewModels;

/// <summary>
/// Evaluación de usabilidad de la app con el System Usability Scale (SUS),
/// al cierre del piloto (después de la encuesta final). Los 10 ítems se
/// presentan en su orden original — alternan afirmaciones positivas e
/// inversas a propósito, y la API puntúa según esa posición. Las dos
/// preguntas abiertas del final son opcionales y aportan la parte
/// cualitativa del enfoque mixto de la tesis.
/// </summary>
public partial class EncuestaSusViewModel : BaseViewModel
{
    // Versión en español del SUS (Brooke, 1996). Confirmar con el asesor la
    // traducción validada que se citará en la tesis.
    private static readonly string[] TextosSus =
    [
        "Creo que me gustaría usar esta aplicación con frecuencia.",
        "Encontré la aplicación innecesariamente compleja.",
        "Pensé que la aplicación era fácil de usar.",
        "Creo que necesitaría el apoyo de una persona con conocimientos técnicos para poder usar esta aplicación.",
        "Encontré que las diversas funciones de esta aplicación estaban bien integradas.",
        "Pensé que había demasiada inconsistencia en esta aplicación.",
        "Imagino que la mayoría de las personas aprenderían a usar esta aplicación muy rápidamente.",
        "Encontré la aplicación muy incómoda de usar.",
        "Me sentí muy seguro(a) usando la aplicación.",
        "Necesité aprender muchas cosas antes de poder empezar a usar esta aplicación.",
    ];

    private readonly IEncuestaSusService encuestaSusService;
    private readonly IAuthService authService;
    private readonly ISincronizacionService sincronizacion;

    public ObservableCollection<PreguntaSusItem> Preguntas { get; }

    [ObservableProperty] private string comentarioLoMasUtil = string.Empty;
    [ObservableProperty] private string comentarioMejoras = string.Empty;
    [ObservableProperty] private string mensajeError = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MostrarFormulario))]
    private bool enviado;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MostrarFormulario))]
    private bool accesoBloqueado;

    [ObservableProperty] private string mensajeBloqueo = string.Empty;

    public bool MostrarFormulario => !Enviado && !AccesoBloqueado;

    public EncuestaSusViewModel(IEncuestaSusService encuestaSusService, IAuthService authService, ISincronizacionService sincronizacion)
    {
        this.encuestaSusService = encuestaSusService;
        this.authService = authService;
        this.sincronizacion = sincronizacion;
        Title = "Evalúa la app";
        Preguntas = new ObservableCollection<PreguntaSusItem>(TextosSus.Select((t, i) => new PreguntaSusItem(i + 1, t)));
    }

    /// <summary>Usa lo que el celular ya sabe; la API vuelve a validar al enviar.</summary>
    public void Inicializar()
    {
        var datos = sincronizacion.Actual;
        AccesoBloqueado = !datos.SusDisponible;
        MensajeBloqueo = datos.SusCompletada
            ? "Ya evaluaste la app. ¡Muchas gracias por tu tiempo!"
            : "La evaluación de la app se habilita después de la encuesta de cierre.";
    }

    [RelayCommand]
    private async Task EnviarAsync()
    {
        if (IsBusy) return;
        MensajeError = string.Empty;

        if (Preguntas.Any(p => p.IndiceRespuesta < 0))
        {
            MensajeError = "Responde las 10 afirmaciones (las preguntas abiertas son opcionales).";
            return;
        }

        try
        {
            IsBusy = true;
            await encuestaSusService.RegistrarAsync(new EncuestaSusRequest
            {
                Respuestas = [.. Preguntas.Select(p => p.IndiceRespuesta + 1)],
                ComentarioLoMasUtil = ComentarioLoMasUtil,
                ComentarioMejoras = ComentarioMejoras,
            });
            sincronizacion.RegistrarSus();
            Enviado = true;
        }
        catch (SesionExpiradaException)
        {
            await authService.CerrarSesionAsync();
            await Shell.Current.DisplayAlertAsync("Sesión expirada", "Tu sesión expiró. Vuelve a iniciar sesión.", "OK");
            await Shell.Current.GoToAsync("//LoginPage");
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        catch (Exception)
        {
            MensajeError = "No se pudo enviar la evaluación. Revisa tu conexión y vuelve a intentar.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private static async Task VolverAsync() => await Shell.Current.GoToAsync("..");
}
