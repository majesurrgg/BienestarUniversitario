using BienestarApp.Models;
using BienestarApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BienestarApp.ViewModels;

/// <summary>
/// Pantalla principal tras el login. Decide el orden obligatorio del
/// estudio: la encuesta basal (fase Basal) se completa una sola vez, antes
/// de que se habilite el check-in diario — no tiene sentido medir el "día a
/// día" sin la línea base con la que se va a comparar. Al cierre del
/// piloto muestra la encuesta final y, después, la evaluación de la app (SUS).
///
/// Es la pantalla que más se abre (se vuelve a ella desde todas las demás),
/// así que NO consulta la API cada vez que aparece: usa
/// <see cref="ISincronizacionService"/>, que consulta como mucho una vez al
/// día — cada consulta despierta la base de Azure y consume el cupo gratuito.
/// </summary>
public partial class MainViewModel : BaseViewModel
{
    private readonly IAuthService authService;
    private readonly ISincronizacionService sincronizacion;
    private readonly IRecordatorioService recordatorioService;

    [ObservableProperty]
    private string subtitulo = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PuedeHacerCheckIn))]
    private bool encuestaBasalCompletada;

    [ObservableProperty]
    private bool yaHizoCheckInHoy;

    [ObservableProperty]
    private int rachaDias;

    [ObservableProperty]
    private bool encuestaFinalDisponible;

    [ObservableProperty]
    private bool susDisponible;

    [ObservableProperty]
    private string mensajeConexion = string.Empty;

    [ObservableProperty]
    private string estadoRecordatorio = string.Empty;

    public bool PuedeHacerCheckIn => EncuestaBasalCompletada;

    public MainViewModel(IAuthService authService, ISincronizacionService sincronizacion, IRecordatorioService recordatorioService)
    {
        this.authService = authService;
        this.sincronizacion = sincronizacion;
        this.recordatorioService = recordatorioService;
        Title = "Bienestar Universitario";
    }

    public async Task InicializarAsync()
    {
        var nombre = await authService.ObtenerNombreAsync();
        Title = string.IsNullOrEmpty(nombre) ? "Bienestar Universitario" : $"Hola, {nombre}";
        Subtitulo = Saludo();

        // La sesión se revisa en el celular (fecha de vencimiento del token),
        // sin llamar a la API.
        if (!await authService.HaySesionActivaAsync())
        {
            await VolverAlLoginAsync();
            return;
        }

        DatosLocales datos;
        try
        {
            datos = await sincronizacion.ObtenerAsync();
            MensajeConexion = string.Empty;
        }
        catch (SesionExpiradaException)
        {
            await VolverAlLoginAsync();
            return;
        }
        catch
        {
            // Sin conexión: se sigue con lo último que se sabía.
            datos = sincronizacion.Actual;
            MensajeConexion = datos.UltimaSincronizacion is null
                ? "No se pudo conectar con el servidor. Revisa tu internet y vuelve a abrir la app."
                : string.Empty;
        }

        var hoy = DateOnly.FromDateTime(DateTime.Now);
        EncuestaBasalCompletada = datos.BasalCompletada;
        YaHizoCheckInHoy = datos.CheckInDe(hoy) is not null;
        RachaDias = CalculadorRacha.Calcular([.. datos.Historial.Select(r => r.Fecha)]);
        EncuestaFinalDisponible = datos.FinalDisponible(hoy);
        SusDisponible = datos.SusDisponible;

        // Cada vez que se abre la pantalla principal se reevalúa el
        // recordatorio de hoy (no consulta la API: usa YaHizoCheckInHoy).
        try
        {
            EstadoRecordatorio = await recordatorioService.ProgramarSiFaltaAsync(YaHizoCheckInHoy);
        }
        catch (Exception ex)
        {
            EstadoRecordatorio = $"No se pudo evaluar el recordatorio: {ex.Message}";
        }
    }

    private static string Saludo()
    {
        var hora = DateTime.Now.Hour;
        return hora switch
        {
            >= 5 and < 12 => "Buenos días. Un minuto para revisar cómo estás hoy.",
            >= 12 and < 19 => "Buenas tardes. Un minuto para revisar cómo estás hoy.",
            _ => "Buenas noches. Un minuto para revisar cómo estuvo tu día.",
        };
    }

    [RelayCommand]
    private static async Task IrACheckInAsync() => await Shell.Current.GoToAsync(nameof(Views.CheckInPage));

    [RelayCommand]
    private static async Task IrAProgresoAsync() => await Shell.Current.GoToAsync(nameof(Views.ProgresoPage));

    [RelayCommand]
    private static async Task IrAEncuestaBasalAsync() => await Shell.Current.GoToAsync(nameof(Views.EncuestaBasalPage));

    [RelayCommand]
    private static async Task IrAEncuestaSusAsync() => await Shell.Current.GoToAsync(nameof(Views.EncuestaSusPage));

    [RelayCommand]
    private static async Task IrARespirarAsync() => await Shell.Current.GoToAsync(nameof(Views.RespiraPage));

    [RelayCommand]
    private async Task CerrarSesionAsync() => await VolverAlLoginAsync();

    private async Task VolverAlLoginAsync()
    {
        await authService.CerrarSesionAsync();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
