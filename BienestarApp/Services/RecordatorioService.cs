using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;

namespace BienestarApp.Services;

/// <summary>
/// Implementación con Plugin.LocalNotification (notificaciones locales del
/// propio celular, sin servidor de push). Se (re)programa o se cancela cada
/// vez que la app conoce el estado del check-in de hoy — al abrir MainPage
/// o CheckInPage, y justo después de guardar un check-in.
///
/// Limitación conocida y aceptada para el piloto: si el estudiante no abre
/// la app en todo el día, nunca se programa el recordatorio de ese día,
/// porque no hay nada del lado del servidor decidiéndolo por su cuenta. La
/// alternativa (notificación push real) necesitaría Firebase Cloud
/// Messaging y un job programado en BienestarApi — se puede evaluar para
/// una versión futura si el piloto muestra que hace falta.
/// </summary>
public class RecordatorioService : IRecordatorioService
{
    private const int NotificationId = 5001;
    private const int HoraRecordatorio = 20; // 8:00 p. m., hora local del celular.

    public async Task ProgramarSiFaltaAsync(bool yaHizoCheckInHoy)
    {
        if (yaHizoCheckInHoy)
        {
            CancelarDeHoy();
            return;
        }

        var permitido = await LocalNotificationCenter.Current.RequestNotificationPermission();
        if (!permitido) return; // el estudiante no dio permiso de notificaciones; no insistimos aquí.

        var ahora = DateTime.Now;
        var horaDisparo = new DateTime(ahora.Year, ahora.Month, ahora.Day, HoraRecordatorio, 0, 0);
        if (horaDisparo <= ahora)
            horaDisparo = horaDisparo.AddDays(1); // ya pasaron las 8pm hoy: recuerda mañana a esa hora.

        // Volver a llamar a Show con el mismo NotificationId reemplaza el
        // recordatorio anterior — no hace falta cancelar antes.
        await LocalNotificationCenter.Current.Show(new NotificationRequest
        {
            NotificationId = NotificationId,
            Title = "¿Cómo estuvo tu día?",
            Description = "Todavía no registraste tu check-in de hoy en Bienestar Universitario.",
            Schedule = new NotificationRequestSchedule { NotifyTime = horaDisparo },
        });
    }

    public void CancelarDeHoy() => LocalNotificationCenter.Current.Cancel(NotificationId);
}
