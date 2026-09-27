using BienestarApp.Models;
using BienestarApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BienestarApp.ViewModels;

/// <summary>
/// Pantalla principal tras el login. Decide el orden obligatorio del
/// estudio: la encuesta basal (fase Basal) se completa una sola vez, antes
/// de que se habilite el check-in diario — no tiene sentido medir el "día a
/// día" sin la línea base con la que se va a comparar.
/// </summary>
public partial class MainViewModel : BaseViewModel
{
    private readonly IAuthService authService;
    private readonly IApiService apiService;
    private readonly IRegistroDiarioService registroDiarioService;
    private readonly IEncuestaBasalService encuestaBasalService;
    private readonly IRecordatorioService recordatorioService;

    [ObservableProperty]
    private string estadoSesion = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PuedeHacerCheckIn))]
    private bool encuestaBasalCompletada;

    [ObservableProperty]
    private string estadoRecordatorio = string.Empty;

    public bool PuedeHacerCheckIn => EncuestaBasalCompletada;

    public MainViewModel(
        IAuthService authService,
        IApiService apiService,
        IRegistroDiarioService registroDiarioService,
        IEncuestaBasalService encuestaBasalService,
        IRecordatorioService recordatorioService)
    {
        this.authService = authService;
        this.apiService = apiService;
        this.registroDiarioService = registroDiarioService;
        this.encuestaBasalService = encuestaBasalService;
        this.recordatorioService = recordatorioService;
        Title = "Bienestar Universitario";
    }

    public async Task InicializarAsync()
    {
        var nombre = await authService.ObtenerNombreAsync();
        Title = string.IsNullOrEmpty(nombre) ? "Bienestar Universitario" : $"Hola, {nombre}";

        var token = await authService.ObtenerTokenAsync();
        if (string.IsNullOrEmpty(token))
        {
            EstadoSesion = "Sin sesión activa.";
            return;
        }

        try
        {
            var valido = await apiService.VerificarSesionAsync(token);
            EstadoSesion = valido
                ? "Sesión verificada contra la API (endpoint protegido con JWT)."
                : "El token guardado ya no es válido.";
        }
        catch
        {
            EstadoSesion = "No se pudo contactar a la API para verificar la sesión.";
        }

        try
        {
            var fases = await encuestaBasalService.ObtenerFasesCompletadasAsync();
            EncuestaBasalCompletada = fases.Contains(FaseEncuesta.Basal);
        }
        catch
        {
            EncuestaBasalCompletada = false;
        }

        // Cada vez que se abre la pantalla principal se reevalúa el
        // recordatorio de hoy — es el punto de entrada más frecuente de la
        // app, así que es donde más chance hay de programarlo a tiempo. El
        // texto que devuelve es de diagnóstico (Sprint 4, quitar en Sprint
        // final): sirve para confirmar en pantalla si quedó programado, sin
        // depender del celular sonando en el momento exacto de probar.
        try
        {
            var deHoy = EncuestaBasalCompletada ? await registroDiarioService.ObtenerDeHoyAsync() : null;
            EstadoRecordatorio = await recordatorioService.ProgramarSiFaltaAsync(deHoy is not null);
        }
        catch (Exception ex)
        {
            EstadoRecordatorio = $"No se pudo evaluar el recordatorio: {ex.Message}";
        }
    }

    [RelayCommand]
    private static async Task IrACheckInAsync() => await Shell.Current.GoToAsync(nameof(Views.CheckInPage));

    [RelayCommand]
    private static async Task IrAProgresoAsync() => await Shell.Current.GoToAsync(nameof(Views.ProgresoPage));

    [RelayCommand]
    private static async Task IrAEncuestaBasalAsync() => await Shell.Current.GoToAsync(nameof(Views.EncuestaBasalPage));

    [RelayCommand]
    private async Task CerrarSesionAsync()
    {
        await authService.CerrarSesionAsync();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
