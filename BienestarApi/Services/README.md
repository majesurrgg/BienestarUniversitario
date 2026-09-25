# Services (BienestarApi)

Carpeta reservada para la lógica de aplicación que los Controllers invocan
(los Controllers no deben hablar directamente con el DbContext salvo casos
triviales; en Sprint 2+ delegan aquí):

- `ITokenService` / `TokenService`: genera y firma los JWT en el login.
- `IAuthService` / `AuthService`: valida credenciales contra `Cuenta`
  (hash de contraseña) y orquesta el login/registro.
- Más adelante: servicios de negocio para calcular alertas de riesgo
  (protocolo del ítem 9 del PHQ-9), agregaciones para reportes, etc.

Sprint 1: carpeta vacía (solo estructura, sin lógica de negocio ni login
funcional — eso corresponde a Sprint 2).

BienestarApp (.NET MAUI, solo Android)
BienestarApi (.NET 8 Web API, solo backend)
