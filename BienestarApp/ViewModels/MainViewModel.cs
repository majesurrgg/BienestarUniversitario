using BienestarApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BienestarApp.ViewModels;

/// <summary>
/// Pantalla principal tras el login. Al aparecer (ver MainPage.xaml.cs)
/// pide el token guardado y llama a un endpoint protegido, solo para
/// demostrar que el circuito completo funciona: login -> token guardado
/// -> header Authorization -> [Authorize] del backend lo acepta.
/// </summary>
public partial class MainViewModel : BaseViewModel
{
    private readonly IAuthService authService;
    private readonly IApiService apiService;
    private readonly IRegistroDiarioService registroDiarioService;
    private readonly IRecordatorioService recordatorioService;

    [ObservableProperty]
    private string estadoSesion = string.Empty;

    public MainViewModel(
        IAuthService authService,
        IApiService apiService,
        IRegistroDiarioService registroDiarioService,
        IRecordatorioService recordatorioService)
    {
        this.authService = authService;
        this.apiService = apiService;
        this.registroDiarioService = registroDiarioService;
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

        // Cada vez que se abre la pantalla principal se reevalúa el
        // recordatorio de hoy — es el punto de entrada más frecuente de la
        // app, así que es donde más chance hay de programarlo a tiempo.
        try
        {
            var deHoy = await registroDiarioService.ObtenerDeHoyAsync();
            await recordatorioService.ProgramarSiFaltaAsync(deHoy is not null);
        }
        catch
        {
            // No crítico: si falla, CheckInPage lo vuelve a intentar cuando se visite.
        }
    }

    [RelayCommand]
    private static async Task IrACheckInAsync() => await Shell.Current.GoToAsync(nameof(Views.CheckInPage));

    [RelayCommand]
    private static async Task IrAEncuestaBasalAsync() => await Shell.Current.GoToAsync(nameof(Views.EncuestaBasalPage));

    [RelayCommand]
    private async Task CerrarSesionAsync()
    {
        await authService.CerrarSesionAsync();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
