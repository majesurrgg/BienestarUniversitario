using BienestarApp.Models;

namespace BienestarApp.Services;

/// <summary>Guarda en el celular la copia de <see cref="DatosLocales"/> del estudiante con sesión iniciada.</summary>
public interface IDatosLocalesService
{
    DatosLocales Obtener();
    void Guardar(DatosLocales datos);

    /// <summary>Se llama al cerrar o iniciar sesión, para no mezclar datos de dos cuentas en el mismo celular.</summary>
    void Borrar();
}
