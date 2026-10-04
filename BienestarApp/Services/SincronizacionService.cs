using BienestarApp.Models;

namespace BienestarApp.Services;

public class SincronizacionService(IApiService apiService, IAuthService authService, IDatosLocalesService datosLocales)
    : ISincronizacionService
{
    // Suficiente para la racha y "Mi progreso" (el piloto dura 4 semanas).
    private const int DiasHistorial = 30;

    private static DateOnly Hoy => DateOnly.FromDateTime(DateTime.Now);

    public DatosLocales Actual => datosLocales.Obtener();

    public async Task<DatosLocales> ObtenerAsync(bool forzar = false)
    {
        var datos = datosLocales.Obtener();
        if (!forzar && datos.UltimaSincronizacion == Hoy)
            return datos;

        var token = await ObtenerTokenAsync();
        var estado = await apiService.ObtenerEstadoEncuestasAsync(token);
        AplicarEstado(datos, estado);

        // Sin basal no hay check-ins que traer: se ahorra la consulta.
        datos.Historial = estado.BasalCompletada
            ? await apiService.ObtenerHistorialRegistroDiarioAsync(token, DiasHistorial)
            : [];
        datos.UltimaSincronizacion = Hoy;

        datosLocales.Guardar(datos);
        return datos;
    }

    public async Task<DatosLocales> ActualizarEstadoEncuestasAsync()
    {
        var datos = datosLocales.Obtener();
        AplicarEstado(datos, await apiService.ObtenerEstadoEncuestasAsync(await ObtenerTokenAsync()));
        datosLocales.Guardar(datos);
        return datos;
    }

    public void RegistrarCheckIn(RegistroDiarioResponse registro)
    {
        var datos = datosLocales.Obtener();
        datos.Historial.RemoveAll(r => r.Fecha == registro.Fecha);
        datos.Historial.Insert(0, registro);
        datosLocales.Guardar(datos);
    }

    public void RegistrarEncuesta(FaseEncuesta fase)
    {
        var datos = datosLocales.Obtener();
        if (fase == FaseEncuesta.Basal) datos.BasalCompletada = true;
        else datos.FinalCompletada = true;
        datosLocales.Guardar(datos);
    }

    public void RegistrarSus()
    {
        var datos = datosLocales.Obtener();
        datos.SusCompletada = true;
        datosLocales.Guardar(datos);
    }

    private static void AplicarEstado(DatosLocales datos, EstadoEncuestasResponse estado)
    {
        datos.BasalCompletada = estado.BasalCompletada;
        datos.FinalCompletada = estado.FinalCompletada;
        datos.FechaHabilitaFinal = estado.FechaHabilitaFinal;
        datos.SusCompletada = estado.SusCompletada;
    }

    private async Task<string> ObtenerTokenAsync() =>
        await authService.ObtenerTokenAsync() ?? throw new SesionExpiradaException();
}
