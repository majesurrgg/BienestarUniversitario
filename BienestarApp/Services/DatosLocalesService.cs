using System.Text.Json;
using BienestarApp.Models;

namespace BienestarApp.Services;

/// <summary>
/// Implementación con Preferences (almacenamiento local de la app). No va en
/// SecureStorage porque no son credenciales: son las mismas respuestas que
/// el estudiante ya ve en pantalla, y Android las borra al desinstalar.
/// </summary>
public class DatosLocalesService : IDatosLocalesService
{
    private const string Clave = "datos_locales";

    public DatosLocales Obtener()
    {
        var json = Preferences.Default.Get(Clave, string.Empty);
        if (string.IsNullOrEmpty(json))
            return new DatosLocales();

        try
        {
            return JsonSerializer.Deserialize<DatosLocales>(json) ?? new DatosLocales();
        }
        catch (JsonException)
        {
            // Formato viejo o dañado: se descarta y la próxima sincronización lo rehace.
            return new DatosLocales();
        }
    }

    public void Guardar(DatosLocales datos) =>
        Preferences.Default.Set(Clave, JsonSerializer.Serialize(datos));

    public void Borrar() => Preferences.Default.Remove(Clave);
}
