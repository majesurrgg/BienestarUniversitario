using BienestarApp.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BienestarApp.ViewModels;

/// <summary>
/// Bienvenida de 3 diapositivas que se ve SOLO la primera vez que se abre
/// la app en el celular (antes del login): qué es, cómo funciona el piloto
/// y cómo se cuidan sus datos. Se recuerda con Preferences, sin API.
/// </summary>
public partial class BienvenidaViewModel : BaseViewModel
{
    private const string ClaveVista = "bienvenida_vista";

    public static bool YaSeVio => Preferences.Default.Get(ClaveVista, false);

    public List<DiapositivaItem> Diapositivas { get; } =
    [
        new()
        {
            Emoji = "💚",
            Titulo = "Bienvenido/a a Bienestar Universitario",
            Texto = "Un espacio para registrar cómo estás cada día y conocerte un poco mejor. " +
                    "Es parte de una investigación de tesis de la Universidad Tecnológica del Perú.",
        },
        new()
        {
            Emoji = "🗓️",
            Titulo = "¿Cómo funciona?",
            Texto = "Al inicio respondes una encuesta de unos 10 minutos. Luego, durante 4 semanas, " +
                    "registras tu día en solo 1 minuto. Al final, una encuesta de cierre.",
        },
        new()
        {
            Emoji = "🔒",
            Titulo = "Tus datos están protegidos",
            Texto = "Tu participación es voluntaria y confidencial, y puedes retirarte cuando quieras. " +
                    "¡Y cada día que registras participas en los sorteos! 🎁",
        },
    ];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EsUltima))]
    [NotifyPropertyChangedFor(nameof(TextoBoton))]
    private int posicion;

    public bool EsUltima => Posicion >= Diapositivas.Count - 1;

    public string TextoBoton => EsUltima ? "Comenzar" : "Siguiente";

    public BienvenidaViewModel()
    {
        Title = "Bienvenida";
    }

    /// <summary>
    /// Se marca como vista apenas aparece (no al final): si la persona sale
    /// con "atrás", no vuelve a salirle cada vez que abre la app.
    /// </summary>
    public static void MarcarVista() => Preferences.Default.Set(ClaveVista, true);

    [RelayCommand]
    private async Task SiguienteAsync()
    {
        if (EsUltima)
            await TerminarAsync();
        else
            Posicion++;
    }

    [RelayCommand]
    private static async Task TerminarAsync() => await Shell.Current.GoToAsync("..");
}
