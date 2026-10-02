using System.Collections.ObjectModel;
using BienestarApp.Models;
using BienestarApp.Services;
using BienestarApp.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BienestarApp.ViewModels;

/// <summary>
/// Check-in diario: el estudiante registra estrés, sueño, actividad física y
/// ánimo una vez al día. Al aparecer, revisa si ya hizo el de hoy con lo que
/// el celular ya sabe (<see cref="ISincronizacionService"/>), sin consultar
/// la API; si ya lo hizo, muestra el resumen en vez del formulario.
/// </summary>
public partial class CheckInViewModel : BaseViewModel
{
    private readonly IRegistroDiarioService registroDiarioService;
    private readonly IAuthService authService;
    private readonly IRecordatorioService recordatorioService;
    private readonly ISincronizacionService sincronizacion;

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

    /// <summary>
    /// true solo justo después de guardar (no al abrir la pantalla con el
    /// día ya registrado): la Vista lo usa para la pequeña celebración.
    /// </summary>
    [ObservableProperty]
    private bool recienGuardado;

    /// <summary>Estrés de hoy en 7 o más: se sugiere el ejercicio de respiración.</summary>
    [ObservableProperty]
    private bool estresAltoHoy;

    /// <summary>Las 5 caritas del ánimo (reemplazan al deslizador: más rápido y más claro).</summary>
    public ObservableCollection<OpcionAnimoItem> OpcionesAnimo { get; }

    public CheckInViewModel(
        IRegistroDiarioService registroDiarioService,
        IAuthService authService,
        IRecordatorioService recordatorioService,
        ISincronizacionService sincronizacion)
    {
        this.registroDiarioService = registroDiarioService;
        this.authService = authService;
        this.recordatorioService = recordatorioService;
        this.sincronizacion = sincronizacion;
        Title = "Mi día";

        OpcionesAnimo =
        [
            new(1, "😞", "Muy malo", SeleccionarAnimo),
            new(2, "😕", "Malo", SeleccionarAnimo),
            new(3, "😐", "Regular", SeleccionarAnimo),
            new(4, "🙂", "Bueno", SeleccionarAnimo),
            new(5, "😄", "Muy bueno", SeleccionarAnimo),
        ];
        MarcarAnimoSeleccionado();
    }

    private void SeleccionarAnimo(OpcionAnimoItem opcion)
    {
        EstadoAnimo = opcion.Valor;
        MarcarAnimoSeleccionado();
    }

    private void MarcarAnimoSeleccionado()
    {
        foreach (var opcion in OpcionesAnimo)
            opcion.Seleccionado = opcion.Valor == (int)EstadoAnimo;
    }

    public bool MostrarFormulario => !YaRegistroHoy;

    public string DescripcionSueno => DescribirEscala5((int)CalidadSueno, "Muy mala", "Mala", "Regular", "Buena", "Muy buena");

    public string DescripcionAnimo => DescribirEscala5((int)EstadoAnimo, "Muy malo", "Malo", "Regular", "Bueno", "Muy bueno");

    public async Task InicializarAsync()
    {
        MensajeError = string.Empty;
        RecienGuardado = false;
        try
        {
            IsBusy = true;
            // Normalmente no consulta nada: la pantalla principal ya
            // sincronizó hoy. Solo llama a la API si todavía no lo hizo.
            var datos = await sincronizacion.ObtenerAsync();
            MostrarResultado(datos.CheckInDe(DateOnly.FromDateTime(DateTime.Now)));
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
            sincronizacion.RegistrarCheckIn(guardado);
            MostrarResultado(guardado);
            RecienGuardado = true;
        }
        catch (SesionExpiradaException)
        {
            await VolverAlLoginAsync();
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
            // Si la API dice que ya había uno hoy (ej. hecho desde otro
            // celular), se trae el real para mostrar el resumen correcto.
            await ReintentarMostrarDeHoyAsync();
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

    private async Task ReintentarMostrarDeHoyAsync()
    {
        try
        {
            var deHoy = await registroDiarioService.ObtenerDeHoyAsync();
            if (deHoy is null) return;
            sincronizacion.RegistrarCheckIn(deHoy);
            MostrarResultado(deHoy);
            MensajeError = string.Empty;
        }
        catch
        {
            // Se queda el mensaje de error original.
        }
    }

    partial void OnNivelEstresChanged(double value) => NivelEstres = Math.Round(value);
    partial void OnCalidadSuenoChanged(double value) => CalidadSueno = Math.Round(value);
    partial void OnEstadoAnimoChanged(double value) => EstadoAnimo = Math.Round(value);

    private void MostrarResultado(RegistroDiarioResponse? registro)
    {
        YaRegistroHoy = registro is not null;

        // Fire-and-forget: si falla programar/cancelar el recordatorio (ej.
        // el celular no dio permiso de notificaciones) no debe romper la
        // pantalla de check-in, que ya cumplió su trabajo principal.
        _ = recordatorioService.ProgramarSiFaltaAsync(registro is not null);

        EstresAltoHoy = registro?.NivelEstres >= 7;
        if (registro is null) return;

        ResumenHoy =
            $"Estrés: {registro.NivelEstres}/10\n" +
            $"Sueño: {registro.CalidadSueno}/5\n" +
            $"Actividad física: {registro.MinutosActividadFisica} min\n" +
            $"Ánimo: {OpcionesAnimo.FirstOrDefault(o => o.Valor == registro.EstadoAnimo)?.Emoji} {registro.EstadoAnimo}/5";
    }

    [RelayCommand]
    private static async Task IrARespiracionAsync() => await Shell.Current.GoToAsync(nameof(Views.RespiracionPage));

    private async Task VolverAlLoginAsync()
    {
        await authService.CerrarSesionAsync();
        await Shell.Current.DisplayAlertAsync("Sesión expirada", "Tu sesión expiró. Vuelve a iniciar sesión.", "OK");
        await Shell.Current.GoToAsync("//LoginPage");
    }

    private static string DescribirEscala5(int valor, params string[] textos) =>
        valor is >= 1 and <= 5 ? textos[valor - 1] : string.Empty;
}
