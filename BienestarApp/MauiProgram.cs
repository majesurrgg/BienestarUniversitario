using BienestarApp.ViewModels;
using BienestarApp.Views;
using Microsoft.Extensions.Logging;

namespace BienestarApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		RegisterAppServices(builder.Services);

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

	// Inyección de dependencias: Vistas y ViewModels se registran aquí para
	// que .NET MAUI resuelva sus constructores automáticamente (constructor
	// injection). En Sprint 2 aquí también se registrarán los Services/
	// (ej. HttpClient tipado hacia BienestarApi, servicio de autenticación).
	private static void RegisterAppServices(IServiceCollection services)
	{
		// Views
		services.AddTransient<MainPage>();

		// ViewModels
		services.AddTransient<MainViewModel>();

		// Services (Sprint 2)
		// services.AddSingleton<IApiService, ApiService>();
		// services.AddSingleton<IAuthService, AuthService>();
	}
}
