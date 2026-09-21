# Services (BienestarApp)

Carpeta reservada para los servicios que los ViewModels consumen (nunca al
revés), por ejemplo:

- `IApiService` / `ApiService`: `HttpClient` tipado que habla con
  BienestarApi (login, envío de RegistroDiario, encuestas, etc.).
- `IAuthService` / `AuthService`: guarda/recupera el JWT (usando
  `SecureStorage` de MAUI) y arma el header `Authorization: Bearer ...`.

Los ViewModels dependen de **interfaces**, no de las implementaciones
concretas, para poder mockearlas en pruebas y para poder registrar las
implementaciones vía inyección de dependencias en `MauiProgram.cs`.

Sprint 1: carpeta vacía (solo estructura, sin lógica de negocio). Se llena
en Sprint 2 junto con el login funcional.
