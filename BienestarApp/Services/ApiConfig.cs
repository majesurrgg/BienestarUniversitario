using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace BienestarApp.Services;

/// <summary>
/// URL base de BienestarApi. "localhost" en el celular/emulador NUNCA es tu
/// PC — es el propio dispositivo — así que se elige según dónde corre la app:
///
///   • Emulador Android: "http://10.0.2.2:5178/" (alias fijo del emulador
///     hacia el localhost de la PC anfitriona).
///   • Celular físico por USB: "http://localhost:5178/" + antes, en una
///     terminal, correr "adb reverse tcp:5178 tcp:5178" (redirige el
///     puerto del celular hacia la PC). Hay que repetirlo cada vez que se
///     desconecta el cable o se reinicia adb.
///   • Celular conectado por WiFi a la "Zona con cobertura inalámbrica
///     móvil" de la PC: "http://192.168.137.1:5178/". Windows le da SIEMPRE
///     esa IP a la PC dentro de su zona móvil, y a los celulares una
///     192.168.137.x; por eso se detecta sola. Requiere que BienestarApi
///     escuche en todas las interfaces (perfil "http" de launchSettings.json)
///     y que el firewall permita el puerto 5178 desde esa red.
///
/// Se usa HTTP (no HTTPS) en desarrollo para no lidiar con certificados
/// autofirmados sin confianza en el emulador/celular; Android lo permite
/// solo para estos hosts (ver Platforms/Android/Resources/xml/
/// network_security_config.xml). Sprint 4: revisar si el piloto necesita
/// HTTPS real.
/// (.NET MAUI, solo Android)
/// </summary>
public static class ApiConfig
{
    private const string UrlEmulador = "http://10.0.2.2:5178/"; // alias fijo del emulador Android hacia el localhost de la PC anfitriona.
    private const string UrlCelularUsb = "http://localhost:5178/"; // dirección para el celular conectado por USB.
    private const string UrlZonaMovilPc = "http://192.168.137.1:5178/"; // la PC dentro de su propia zona móvil.
    private const string PrefijoZonaMovilPc = "192.168.137.";

    // La app se pregunta ¿soy un emulador? ¿estoy en la zona móvil de la PC?
    // y elige la dirección correcta. Se evalúa al abrir la app: si cambias
    // de red (cable <-> WiFi), cierra y vuelve a abrir la app.
    public static string BaseUrl =>
        DeviceInfo.Current.DeviceType == DeviceType.Virtual ? UrlEmulador
        : EstaEnZonaMovilDeLaPc() ? UrlZonaMovilPc
        : UrlCelularUsb;

    // Revisa las IPs del propio celular: si alguna es 192.168.137.x, está
    // conectado al WiFi que comparte la PC.
    private static bool EstaEnZonaMovilDeLaPc()
    {
        try
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(i => i.OperationalStatus == OperationalStatus.Up)
                .SelectMany(i => i.GetIPProperties().UnicastAddresses)
                .Any(a => a.Address.AddressFamily == AddressFamily.InterNetwork
                          && a.Address.ToString().StartsWith(PrefijoZonaMovilPc));
        }
        catch
        {
            // Si Android no deja leer las interfaces, se sigue con la opción por cable.
            return false;
        }
    }
}
