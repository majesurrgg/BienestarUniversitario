using BienestarApp.Views;

namespace BienestarApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Ruta para una página que se apila encima de Login con GoToAsync
        // (ver AppShell.xaml; MainPage ya es raíz ahí).
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        Routing.RegisterRoute(nameof(ConsentimientoPage), typeof(ConsentimientoPage));
        // Se apilan encima de MainPage (Sprint 3).
        Routing.RegisterRoute(nameof(CheckInPage), typeof(CheckInPage));
        Routing.RegisterRoute(nameof(EncuestaBasalPage), typeof(EncuestaBasalPage));
        Routing.RegisterRoute(nameof(ProgresoPage), typeof(ProgresoPage));
        Routing.RegisterRoute(nameof(EncuestaSusPage), typeof(EncuestaSusPage));
    }
}
