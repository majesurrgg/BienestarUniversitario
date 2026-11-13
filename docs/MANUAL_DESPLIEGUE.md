# Manual de despliegue

Este manual describe cómo poner en funcionamiento el sistema Bienestar Universitario: la base de datos, la API REST y la aplicación móvil Android. La sección A sirve para ejecutarlo en una computadora local (desarrollo). La sección B describe el despliegue en Microsoft Azure que se usó durante el piloto.

## Requisitos

| Herramienta | Versión usada |
|---|---|
| .NET SDK | 10.0 |
| Visual Studio 2026 con la carga de trabajo ".NET Multi-platform App UI" | 18.x |
| SQL Server (local) o Azure SQL Database (nube) | SQL Server 2022 / Azure SQL |
| Herramienta `dotnet-ef` | 10.0.11 (incluida en `dotnet-tools.json`) |
| Dispositivo Android o emulador | Android 5.0 (API 21) o superior |
| Power BI Desktop (solo para el dashboard) | versión actual |

Clonar el repositorio y restaurar las herramientas:

```powershell
git clone https://github.com/majesurrgg/BienestarUniversitario.git
cd BienestarUniversitario
dotnet tool restore
```

---

## A. Ejecución local

### A.1 Base de datos

1. Tener una instancia de SQL Server accesible desde la PC. La cadena de conexión por defecto de `BienestarApi/appsettings.json` apunta a `Server=localhost;Database=BienestarDW;Trusted_Connection=True`.
2. Crear las tablas con las migraciones de Entity Framework Core:

   ```powershell
   dotnet ef database update --project BienestarApi
   ```

   Como alternativa sin .NET, ejecutar el script [`database/BienestarDW_esquema.sql`](../database/BienestarDW_esquema.sql) en SQL Server Management Studio o en el Query editor de Azure. Es idempotente: se puede ejecutar más de una vez.

3. Crear las vistas analíticas para Power BI con el script de la carpeta [`database/`](../database) (ver su README).

### A.2 API REST

1. Configurar la clave de firma de los tokens JWT (mínimo 32 caracteres) sin escribirla en el repositorio:

   ```powershell
   cd BienestarApi
   dotnet user-secrets set "Jwt:Key" "<una-clave-secreta-de-al-menos-32-caracteres>"
   ```

2. Opcional: ajustar la sección `Piloto` de `appsettings.json`. `CodigoInvitacion` es el código que pide el registro, `MaxParticipantes` el cupo y `FechaHabilitaEncuestaFinal` la fecha desde la que se habilita la encuesta final.
3. Ejecutar la API con el perfil `http` (escucha en el puerto 5178):

   ```powershell
   dotnet run --launch-profile http
   ```

4. Comprobar que responde: `GET http://localhost:5178/api/health`.

### A.3 Aplicación móvil (modo Debug)

1. Abrir `TesisBienestar.slnx` en Visual Studio y seleccionar el proyecto `BienestarApp` con la configuración **Debug**.
2. En Debug, `BienestarApp/Services/ApiConfig.cs` elige la dirección de la API según dónde corre la app:
   - Emulador Android: `http://10.0.2.2:5178/`.
   - Celular por USB: `http://localhost:5178/`, ejecutando antes `adb reverse tcp:5178 tcp:5178`.
   - Celular conectado a la zona con cobertura móvil de la PC: `http://192.168.137.1:5178/`. El firewall debe permitir el puerto 5178.
3. Ejecutar (F5) sobre el emulador o el dispositivo.

---

## B. Despliegue en Microsoft Azure (piloto)

Durante el piloto la base de datos y la API se alojaron en Azure, en la región West US 2, con los niveles gratuitos.

### B.1 Base de datos (Azure SQL Database)

1. Crear un grupo de recursos y un servidor lógico de Azure SQL con autenticación "SQL y Microsoft Entra".
2. Crear la base de datos `BienestarDW` usando la oferta gratuita de Azure SQL Database (serverless, se pausa sola al llegar al límite gratuito, sin cobros).
3. En *Redes* del servidor:
   - permitir el acceso de los servicios de Azure;
   - agregar una regla de firewall con la IP pública de la PC desde la que se aplicarán las migraciones.
4. Guardar la cadena de conexión de Azure como User Secret (no en `appsettings.json`) y aplicar las migraciones:

   ```powershell
   cd BienestarApi
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=tcp:<servidor>.database.windows.net,1433;Initial Catalog=BienestarDW;User ID=<usuario>;Password=<contraseña>;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
   dotnet ef database update
   ```

   La primera conexión puede fallar con el error transitorio 40613 mientras la base serverless se reanuda. Basta con reintentar después de uno o dos minutos.

5. Crear las vistas analíticas ejecutando su script en *Query editor* de la base.

### B.2 API (Azure App Service)

1. En Visual Studio: clic derecho en `BienestarApi` → **Publicar** → Azure → Azure App Service (Windows) → crear una instancia nueva con un plan de hospedaje **F1 (gratuito)** en la misma región.
2. En Azure Portal → App Service → **Variables de entorno** → *App settings*, crear:

   | Nombre | Valor |
   |---|---|
   | `ConnectionStrings__DefaultConnection` | cadena de conexión a `BienestarDW` |
   | `Jwt__Key` | clave secreta de firma de los tokens |

   El doble guion bajo equivale a la sección anidada de `appsettings.json`.
3. Publicar y comprobar `GET https://<app-service>.azurewebsites.net/api/health`.

El plan F1 no tiene "Always On": tras un periodo sin uso, la primera petición tarda unos segundos más.

### B.3 Aplicación móvil (APK de Release)

1. En `BienestarApp/Services/ApiConfig.cs`, la constante `UrlProduccion` debe apuntar a la URL del App Service. Es la dirección que usa la app compilada en **Release**.
2. En Visual Studio, cambiar la configuración a **Release** y abrir **Archive Manager** → *Distribuir* → *Ad Hoc*. El proyecto ya está configurado para generar `.apk` (`AndroidPackageFormat`).
3. Firmar el APK con un almacén de claves (keystore). La primera vez se crea desde el mismo asistente. Hay que respaldar el keystore y su contraseña, porque sin él no se pueden publicar actualizaciones de la misma app.
4. Distribuir el `.apk` (por ejemplo, con un enlace o un código QR). En el celular se habilita una sola vez "Instalar apps de fuentes desconocidas".

### B.4 Verificación de extremo a extremo

1. `GET /api/health` responde 200.
2. Registrar una cuenta desde la app con el código de invitación.
3. En Azure Portal → `BienestarDW` → **Query editor**, comprobar que aparecen los registros, por ejemplo con `SELECT TOP 10 * FROM RegistrosDiarios;`.

---

## C. Dashboard (Power BI)

1. En Power BI Desktop: *Obtener datos* → *Azure SQL Database*, indicar el servidor y `BienestarDW`, en modo **Importación**.
2. Seleccionar solo las vistas `vw_pbi_*`, no las tablas. Así el dashboard no accede a credenciales ni a datos identificatorios.
3. Actualizar el modelo con *Actualizar* cada vez que se quieran datos nuevos.
