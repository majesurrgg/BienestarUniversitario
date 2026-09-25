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
    }
}
