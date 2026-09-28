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
    private string subtitulo = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PuedeHacerCheckIn))]
    private bool encuestaBasalCompletada;

    [ObservableProperty]
    private bool yaHizoCheckInHoy;

    [ObservableProperty]
    private int rachaDias;

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
        Subtitulo = Saludo();

        var token = await authService.ObtenerTokenAsync();
        if (string.IsNullOrEmpty(token)) return;

        // Verificación silenciosa de la sesión: si el token ya no es
        // válido, hay que sacarlo de la app en vez de dejarlo ver botones
        // que van a fallar apenas los toque. No se le muestra nada de esto
        // al estudiante si todo está bien — no le importa cómo funciona
        // JWT por dentro.
        try
        {
            var valido = await apiService.VerificarSesionAsync(token);
            if (!valido)
            {
                await VolverAlLoginAsync();
                return;
            }
        }
        catch
        {
            // Sin conexión: se deja seguir con lo que ya está en el celular
            // (SecureStorage), no se corta la sesión por un problema de red.
        }

        List<RegistroDiarioResponse> historial = [];
        try
        {
            var fases = await encuestaBasalService.ObtenerFasesCompletadasAsync();
            EncuestaBasalCompletada = fases.Contains(FaseEncuesta.Basal);

            if (EncuestaBasalCompletada)
                historial = await registroDiarioService.ObtenerHistorialAsync(7);
        }
        catch
        {
            EncuestaBasalCompletada = false;
        }

        var hoy = DateOnly.FromDateTime(DateTime.Now);
        YaHizoCheckInHoy = historial.Any(r => r.Fecha == hoy);
        RachaDias = CalculadorRacha.Calcular([.. historial.Select(r => r.Fecha)]);

        // Cada vez que se abre la pantalla principal se reevalúa el
        // recordatorio de hoy — es el punto de entrada más frecuente de la
        // app, así que es donde más chance hay de programarlo a tiempo. El
        // texto que devuelve es de diagnóstico (Sprint 4, quitar en Sprint
        // final): sirve para confirmar en pantalla si quedó programado, sin
        // depender del celular sonando en el momento exacto de probar.
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
    private async Task CerrarSesionAsync() => await VolverAlLoginAsync();

    private async Task VolverAlLoginAsync()
    {
        await authService.CerrarSesionAsync();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
