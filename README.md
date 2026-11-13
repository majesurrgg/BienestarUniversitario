# Bienestar Universitario

Código fuente de la tesis **"Aplicación móvil con Business Intelligence para el monitoreo del bienestar físico y mental en estudiantes universitarios de Arequipa"** (Universidad Tecnológica del Perú, Facultad de Ingeniería, 2026).

El sistema tiene tres partes:

| Componente | Tecnología | Carpeta |
|---|---|---|
| Aplicación móvil (Android) | .NET MAUI 10, patrón MVVM | [`BienestarApp/`](BienestarApp) |
| API REST | ASP.NET Core 10, Entity Framework Core, JWT | [`BienestarApi/`](BienestarApi) |
| Base de datos (Data Warehouse, modelo estrella) | Azure SQL Database | [`database/`](database) y [`BienestarApi/Data/Migrations/`](BienestarApi/Data/Migrations) |

El dashboard se construyó en Power BI Desktop sobre las vistas analíticas de la base. El archivo `.pbix` no se publica porque contiene los datos importados de los participantes del piloto.

## Documentación

- [Manual de despliegue](docs/MANUAL_DESPLIEGUE.md): cómo levantar la base de datos, la API y generar el APK, en local y en Azure.
- [Base de datos](database/README.md): script del esquema, tablas y vistas analíticas.

## Estructura

```
TesisBienestar.slnx        Solución de Visual Studio
BienestarApi/              API REST (Controllers, Services, DTOs, Models, Data)
BienestarApp/              Aplicación móvil (Views, ViewModels, Services, Models)
database/                  Script SQL del esquema de BienestarDW
docs/                      Manual de despliegue
```

## Seguridad

El repositorio no contiene secretos. La cadena de conexión real y la clave de firma JWT se configuran fuera del código: con .NET User Secrets en desarrollo y como variables de entorno del App Service en producción. Los valores de `appsettings.json` son de ejemplo.
