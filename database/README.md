# Base de datos BienestarDW

Base de datos del sistema (Azure SQL Database / SQL Server). Sigue un modelo estrella: dos dimensiones (`DimUsuario`, `DimTiempo`) y tablas de hechos con los registros diarios y las encuestas.

## Archivos

| Archivo | Contenido |
|---|---|
| [`BienestarDW_esquema.sql`](BienestarDW_esquema.sql) | Crea todas las tablas, claves e índices. Se generó con `dotnet ef migrations script --idempotent` a partir de las migraciones de [`BienestarApi/Data/Migrations`](../BienestarApi/Data/Migrations). |
| [`BienestarDW_vistas_powerbi.sql`](BienestarDW_vistas_powerbi.sql) | Vistas analíticas que consume el dashboard de Power BI. |

No se incluyen datos: el repositorio es público y los registros del piloto son datos personales de salud.

## Tablas

| Tabla | Tipo | Descripción |
|---|---|---|
| `DimUsuario` | Dimensión | Participante: nombre, código universitario, carrera y aceptación del consentimiento |
| `DimTiempo` | Dimensión | Calendario: fecha, año, semana y día de la semana |
| `Cuentas` | Seguridad | Credenciales de acceso (correo y hash de la contraseña); no la lee Power BI |
| `RegistrosDiarios` | Hechos | Check-in diario: estrés, sueño, estado de ánimo y actividad física |
| `EncuestasBasal` | Hechos | PHQ-9, SISCO SV-21 e IPAQ, en fase inicial y final |
| `EncuestasSUS` | Hechos | Encuesta de usabilidad SUS |
| `AlertasRiesgo` | Auditoría | Alertas generadas por el ítem 9 del PHQ-9 |

## Vistas para Power BI

| Vista | Uso |
|---|---|
| `vw_pbi_Participante` | Participantes identificados solo por código |
| `vw_pbi_RegistroDiario` | Registros diarios con día y semana del piloto |
| `vw_pbi_Encuesta` | Puntajes de las encuestas basales |
| `vw_pbi_PrePost` | Comparación inicial y final por participante |
| `vw_pbi_SUS` | Puntajes SUS |
| `vw_pbi_Alerta` | Alertas de riesgo |

## Cómo crear la base

```powershell
# opción 1: con las migraciones
dotnet ef database update --project BienestarApi

# opción 2: ejecutar BienestarDW_esquema.sql en SSMS o en el Query editor de Azure
```

Después, ejecutar `BienestarDW_vistas_powerbi.sql`. El procedimiento completo está en el [manual de despliegue](../docs/MANUAL_DESPLIEGUE.md).
