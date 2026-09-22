namespace BienestarApp.Services;

/// <summary>
/// URL base de BienestarApi. "localhost" en el celular/emulador NUNCA es tu
/// PC — es el propio dispositivo — así que hay que elegir según dónde
/// pruebes, cambiando esta constante:
///
///   • Emulador Android: "http://10.0.2.2:5178/" (alias fijo del emulador
///     hacia el localhost de la PC anfitriona). Es el valor por defecto.
///   • Celular físico por USB: "http://localhost:5178/" + antes, en una
///     terminal, correr "adb reverse tcp:5178 tcp:5178" (redirige el
///     puerto del celular hacia la PC).
///   • Celular en la misma red WiFi que la PC: la IP local de la PC, ej.
///     "http://192.168.1.50:5178/" (y correr BienestarApi con
///     `dotnet run --urls http://0.0.0.0:5178` para que escuche en todas
///     las interfaces, no solo localhost).
///
/// Se usa HTTP (no HTTPS) en desarrollo para no lidiar con certificados
/// autofirmados sin confianza en el emulador/celular. Sprint 4: revisar
/// si el piloto necesita HTTPS real.
/// </summary>
public static class ApiConfig
{
    public const string BaseUrl = "http://10.0.2.2:5178/";
}
