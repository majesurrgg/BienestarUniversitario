using BienestarApp.ViewModels;

namespace BienestarApp.Views;

public partial class LoginPage : ContentPage
{
    private const string ArchivoFondo = "fondo_login.jpg";

    // Bytes de la foto, leídos una sola vez y compartidos entre aperturas
    // de la pantalla (se vuelve al login al cerrar sesión).
    private static byte[]? bytesFondo;

    private readonly LoginViewModel viewModel;

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
        _ = CargarFondoAsync();
    }

    // Login es la primera pantalla de Shell: al aparecer, si ya hay sesión
    // guardada, el ViewModel manda directo a la pantalla principal.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = viewModel.EntrarSiHaySesionAsync();
    }

    /// <summary>
    /// La foto vive en Resources/Raw (archivo de la app, JPG comprimido) y se
    /// lee directo del paquete. Como imagen normal de Resources/Images no
    /// llegaba a dibujarse en Android; así además pesa ~0.2 MB en el APK en
    /// vez de ~10 MB. Si fallara, queda el degradado turquesa de respaldo.
    /// </summary>
    private async Task CargarFondoAsync()
    {
        try
        {
            if (bytesFondo is null)
            {
                await using var archivo = await FileSystem.OpenAppPackageFileAsync(ArchivoFondo);
                using var memoria = new MemoryStream();
                await archivo.CopyToAsync(memoria);
                bytesFondo = memoria.ToArray();
            }

            var bytes = bytesFondo;
            FondoImagen.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
        }
        catch
        {
            // Sin foto: se ve el degradado turquesa del Grid.
        }
    }
}
