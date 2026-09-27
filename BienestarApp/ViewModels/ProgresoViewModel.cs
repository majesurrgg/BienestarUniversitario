using System.Collections.ObjectModel;
using BienestarApp.Services;
using BienestarApp.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BienestarApp.ViewModels;

/// <summary>
/// "Mi progreso": lo único que el estudiante recibe a cambio de llenar el
/// check-in todos los días. Sin esto, la app solo pide datos y no devuelve
/// nada — riesgo real de abandono en un piloto de varias semanas.
/// </summary>
public partial class ProgresoViewModel : BaseViewModel
{
    private readonly IRegistroDiarioService registroDiarioService;
    private readonly IAuthService authService;

    public ObservableCollection<DiaProgresoItem> Dias { get; } = [];

    [ObservableProperty] private string mensajeError = string.Empty;
    [ObservableProperty] private bool sinDatos;
    [ObservableProperty] private int rachaDias;
    [ObservableProperty] private double promedioEstres;

    public ProgresoViewModel(IRegistroDiarioService registroDiarioService, IAuthService authService)
    {
        this.registroDiarioService = registroDiarioService;
        this.authService = authService;
        Title = "Mi progreso";
    }

    public async Task InicializarAsync()
    {
        MensajeError = string.Empty;
        try
        {
            IsBusy = true;
            var historial = await registroDiarioService.ObtenerHistorialAsync(7);

            Dias.Clear();
            foreach (var r in historial)
            {
                Dias.Add(new DiaProgresoItem
                {
                    Fecha = r.Fecha,
                    NivelEstres = r.NivelEstres,
                    CalidadSueno = r.CalidadSueno,
                    EstadoAnimo = r.EstadoAnimo,
                    MinutosActividadFisica = r.MinutosActividadFisica,
                });
            }

            SinDatos = Dias.Count == 0;
            PromedioEstres = Dias.Count > 0 ? Math.Round(Dias.Average(d => d.NivelEstres), 1) : 0;
            RachaDias = CalcularRacha([.. historial.Select(r => r.Fecha)]);
        }
        catch (SesionExpiradaException)
        {
            await VolverAlLoginAsync();
        }
        catch (Exception)
        {
            MensajeError = "No se pudo cargar tu progreso. Revisa tu conexión con el servidor.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private static async Task VolverAsync() => await Shell.Current.GoToAsync("..");

    private async Task VolverAlLoginAsync()
    {
        await authService.CerrarSesionAsync();
        await Shell.Current.DisplayAlertAsync("Sesión expirada", "Tu sesión expiró. Vuelve a iniciar sesión.", "OK");
        await Shell.Current.GoToAsync("//LoginPage");
    }

    /// <summary>
    /// Días seguidos con check-in, contando hacia atrás desde hoy o ayer
    /// (si todavía no hizo el de hoy, no corta la racha). <paramref
    /// name="fechasDesc"/> viene ordenada de más reciente a más antigua.
    /// </summary>
    private static int CalcularRacha(List<DateOnly> fechasDesc)
    {
        if (fechasDesc.Count == 0) return 0;

        var hoy = DateOnly.FromDateTime(DateTime.Now);
        var esperado = fechasDesc[0];
        if (esperado != hoy && esperado != hoy.AddDays(-1)) return 0;

        var racha = 0;
        foreach (var fecha in fechasDesc)
        {
            if (fecha != esperado) break;
            racha++;
            esperado = esperado.AddDays(-1);
        }
        return racha;
    }
}
