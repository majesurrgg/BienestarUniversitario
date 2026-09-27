using BienestarApp.Models;
using BienestarApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BienestarApp.ViewModels;

/// <summary>
/// Check-in diario: el estudiante registra estrés, sueño, actividad física y
/// ánimo una vez al día. Al aparecer, la pantalla pregunta a la API si ya
/// hizo el de hoy; si ya lo hizo, muestra el resumen en vez del formulario.
/// </summary>
public partial class CheckInViewModel : BaseViewModel
{
    private readonly IRegistroDiarioService registroDiarioService;
    private readonly IAuthService authService;

    // Los Slider/Stepper de MAUI trabajan con double; se redondean al
    // moverse y se envían como int (las escalas son enteras).
    [ObservableProperty]
    private double nivelEstres = 5;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DescripcionSueno))]
    private double calidadSueno = 3;

    [ObservableProperty]
    private double minutosActividadFisica;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DescripcionAnimo))]
    private double estadoAnimo = 3;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MostrarFormulario))]
    private bool yaRegistroHoy;

    [ObservableProperty]
    private string resumenHoy = string.Empty;

    [ObservableProperty]
    private string mensajeError = string.Empty;

    public CheckInViewModel(IRegistroDiarioService registroDiarioService, IAuthService authService)
    {
        this.registroDiarioService = registroDiarioService;
        this.authService = authService;
        Title = "Mi día";
    }

    public bool MostrarFormulario => !YaRegistroHoy;

    public string DescripcionSueno => DescribirEscala5((int)CalidadSueno, "Muy mala", "Mala", "Regular", "Buena", "Muy buena");

    public string DescripcionAnimo => DescribirEscala5((int)EstadoAnimo, "Muy malo", "Malo", "Regular", "Bueno", "Muy bueno");

    public async Task InicializarAsync()
    {
        MensajeError = string.Empty;
        try
        {
            IsBusy = true;
            var deHoy = await registroDiarioService.ObtenerDeHoyAsync();
            MostrarResultado(deHoy);
        }
        catch (SesionExpiradaException)
        {
            await VolverAlLoginAsync();
        }
        catch (Exception)
        {
            MensajeError = "No se pudo conectar con el servidor. Revisa tu conexión y la URL de la API.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (IsBusy) return;

        MensajeError = string.Empty;
        try
        {
            IsBusy = true;
            var guardado = await registroDiarioService.RegistrarHoyAsync(new RegistroDiarioRequest
            {
                NivelEstres = (int)NivelEstres,
                CalidadSueno = (int)CalidadSueno,
                MinutosActividadFisica = (int)MinutosActividadFisica,
                EstadoAnimo = (int)EstadoAnimo,
            });
            MostrarResultado(guardado);
        }
        catch (SesionExpiradaException)
        {
            await VolverAlLoginAsync();
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        catch (Exception)
        {
            MensajeError = "No se pudo conectar con el servidor. Revisa tu conexión y la URL de la API.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnNivelEstresChanged(double value) => NivelEstres = Math.Round(value);
    partial void OnCalidadSuenoChanged(double value) => CalidadSueno = Math.Round(value);
    partial void OnEstadoAnimoChanged(double value) => EstadoAnimo = Math.Round(value);

    private void MostrarResultado(RegistroDiarioResponse? registro)
    {
        YaRegistroHoy = registro is not null;
        if (registro is null) return;

        ResumenHoy =
            $"Estrés: {registro.NivelEstres}/10\n" +
            $"Sueño: {registro.CalidadSueno}/5\n" +
            $"Actividad física: {registro.MinutosActividadFisica} min\n" +
            $"Ánimo: {registro.EstadoAnimo}/5";
    }

    private async Task VolverAlLoginAsync()
    {
        await authService.CerrarSesionAsync();
        await Shell.Current.DisplayAlertAsync("Sesión expirada", "Tu sesión expiró. Vuelve a iniciar sesión.", "OK");
        await Shell.Current.GoToAsync("//LoginPage");
    }

    private static string DescribirEscala5(int valor, params string[] textos) =>
        valor is >= 1 and <= 5 ? textos[valor - 1] : string.Empty;
}
