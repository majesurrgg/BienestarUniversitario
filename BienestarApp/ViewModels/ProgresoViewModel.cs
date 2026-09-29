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
    private const int DiasMostrados = 7;

    private readonly ISincronizacionService sincronizacion;
    private readonly IAuthService authService;

    public ObservableCollection<DiaProgresoItem> Dias { get; } = [];

    [ObservableProperty] private string mensajeError = string.Empty;
    [ObservableProperty] private bool sinDatos;
    [ObservableProperty] private int rachaDias;
    [ObservableProperty] private double promedioEstres;

    public ProgresoViewModel(ISincronizacionService sincronizacion, IAuthService authService)
    {
        this.sincronizacion = sincronizacion;
        this.authService = authService;
        Title = "Mi progreso";
    }

    public async Task InicializarAsync()
    {
        MensajeError = string.Empty;
        try
        {
            IsBusy = true;
            // Usa el historial que el celular ya tiene (se actualiza al
            // guardar cada check-in); solo consulta la API si no se
            // sincronizó hoy.
            var historial = (await sincronizacion.ObtenerAsync()).Historial;
            var desde = DateOnly.FromDateTime(DateTime.Now).AddDays(-(DiasMostrados - 1));

            Dias.Clear();
            foreach (var r in historial.Where(r => r.Fecha >= desde))
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
            RachaDias = CalculadorRacha.Calcular([.. historial.Select(r => r.Fecha)]);
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

}
