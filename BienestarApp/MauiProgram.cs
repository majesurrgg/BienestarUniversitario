using BienestarApp.Services;
using BienestarApp.ViewModels;
using BienestarApp.Views;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;

namespace BienestarApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseLocalNotification()
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
	// injection).
	private static void RegisterAppServices(IServiceCollection services)
	{
		// Views
		services.AddTransient<Views.LoginPage>();
		services.AddTransient<Views.RegisterPage>();
		services.AddTransient<MainPage>();
		services.AddTransient<Views.CheckInPage>();
		services.AddTransient<Views.EncuestaBasalPage>();
		services.AddTransient<Views.ProgresoPage>();

		// ViewModels
		services.AddTransient<LoginViewModel>();
		services.AddTransient<RegisterViewModel>();
		services.AddTransient<MainViewModel>();
		services.AddTransient<CheckInViewModel>();
		services.AddTransient<EncuestaBasalViewModel>();
		services.AddTransient<ProgresoViewModel>();

		// Services: HttpClient tipado hacia BienestarApi + servicios propios.
		// Singleton porque no guardan estado por pantalla (la sesión vive en
		// SecureStorage, no en memoria del servicio).
		services.AddHttpClient<IApiService, ApiService>(client =>
		{
			client.BaseAddress = new Uri(ApiConfig.BaseUrl);
		});
		services.AddSingleton<IAuthService, AuthService>();
		services.AddSingleton<IRegistroDiarioService, RegistroDiarioService>();
		services.AddSingleton<IEncuestaBasalService, EncuestaBasalService>();
		services.AddSingleton<IRecordatorioService, RecordatorioService>();
	}
}
