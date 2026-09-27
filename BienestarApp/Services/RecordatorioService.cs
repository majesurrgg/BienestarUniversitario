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
    private const int HoraRecordatorio = 17; // hora local del celular. TODO: volver a 20 (8pm) después de probar.

    public async Task<string> ProgramarSiFaltaAsync(bool yaHizoCheckInHoy)
    {
        if (yaHizoCheckInHoy)
        {
            CancelarDeHoy();
            return "Ya hiciste tu check-in de hoy: recordatorio cancelado.";
        }

        bool permitido;
        try
        {
            permitido = await LocalNotificationCenter.Current.RequestNotificationPermission();
        }
        catch (Exception ex)
        {
            return $"Error al pedir permiso de notificaciones: {ex.Message}";
        }

        if (!permitido)
            return "Permiso de notificaciones DENEGADO — actívalo en Ajustes del celular > Apps > BienestarApp > Notificaciones.";

        var ahora = DateTime.Now;
        var horaDisparo = new DateTime(ahora.Year, ahora.Month, ahora.Day, HoraRecordatorio, 0, 0);
        if (horaDisparo <= ahora)
            horaDisparo = horaDisparo.AddDays(1);

        try
        {
            var ok = await LocalNotificationCenter.Current.Show(new NotificationRequest
            {
                NotificationId = NotificationId,
                Title = "¿Cómo estuvo tu día?",
                Description = "Todavía no registraste tu check-in de hoy en Bienestar Universitario.",
                Schedule = new NotificationRequestSchedule { NotifyTime = horaDisparo },
            });
            return ok
                ? $"Recordatorio programado para las {horaDisparo:HH:mm} del {horaDisparo:dd/MM}."
                : "LocalNotificationCenter.Show() devolvió false (no se programó).";
        }
        catch (Exception ex)
        {
            return $"Error al programar la notificación: {ex.Message}";
        }
    }

    public void CancelarDeHoy() => LocalNotificationCenter.Current.Cancel(NotificationId);
}
