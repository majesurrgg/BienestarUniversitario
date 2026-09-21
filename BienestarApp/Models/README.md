# Models (BienestarApp)

Carpeta reservada para los modelos/DTOs que usa la app móvil para
**representar datos que vienen o van hacia la API** (BienestarApi), por
ejemplo `UsuarioDto`, `RegistroDiarioDto`, `LoginRequest`, etc.

Estos modelos son **de transporte/presentación**, no las entidades de
EF Core del backend (esas viven en `BienestarApi/Models`). Se mantienen
separados a propósito: la app móvil no debería depender directamente del
modelo de base de datos del servidor, sino de un contrato (DTO) estable.

Sprint 1: carpeta vacía (solo estructura). Se llena en Sprint 2 cuando se
conecte la app a la API.
