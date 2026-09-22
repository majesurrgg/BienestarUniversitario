using BienestarApp.Views;

namespace BienestarApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Rutas para páginas a las que se navega con GoToAsync pero que no
        // son pestañas de la barra de navegación (ver AppShell.xaml).
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
    }
}
