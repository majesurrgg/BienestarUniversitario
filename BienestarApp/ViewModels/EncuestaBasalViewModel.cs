using System.Collections.ObjectModel;
using BienestarApp.Models;
using BienestarApp.Services;
using BienestarApp.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BienestarApp.ViewModels;

/// <summary>
/// Encuesta basal (fase Basal o Final del diseño pre-post): PHQ-9 completo
/// (con el protocolo del ítem 9), IPAQ corto, y SISCO — este último con
/// placeholder hasta confirmar el instrumento exacto con el asesor/comité
/// de ética (ver el TODO en <see cref="PuntajeSisco"/>).
/// </summary>
public partial class EncuestaBasalViewModel : BaseViewModel
{
    private readonly IEncuestaBasalService encuestaBasalService;
    private readonly IAuthService authService;

    // Texto oficial del PHQ-9 (Patient Health Questionnaire-9), instrumento
    // estandarizado y de dominio público — a diferencia de SISCO, no hace
    // falta confirmar versión.
    private static readonly string[] TextosPhq9 =
    [
        "Poco interés o placer en hacer las cosas",
        "Se ha sentido decaído(a), deprimido(a) o sin esperanzas",
        "Dificultad para quedarse o permanecer dormido(a), o ha dormido demasiado",
        "Se ha sentido cansado(a) o con poca energía",
        "Falta de apetito o ha comido en exceso",
        "Se ha sentido mal con usted mismo(a), o que es un fracaso, o que ha quedado mal con usted o su familia",
        "Dificultad para concentrarse en cosas, como leer o ver televisión",
        "Se ha sentido tan lento(a) o inquieto(a) que otras personas lo han notado",
        "Pensamientos de que estaría mejor muerto(a), o de hacerse daño de alguna manera",
    ];

    public ObservableCollection<PreguntaPhq9Item> PreguntasPhq9 { get; }

    public List<string> FasesDisponibles { get; } = ["Basal", "Final"];

    [ObservableProperty]
    private string faseSeleccionada = "Basal";

    // --- IPAQ corto (International Physical Activity Questionnaire) ---
    [ObservableProperty] private string ipaqVigorosoDias = "0";
    [ObservableProperty] private string ipaqVigorosoMinutos = "0";
    [ObservableProperty] private string ipaqModeradoDias = "0";
    [ObservableProperty] private string ipaqModeradoMinutos = "0";
    [ObservableProperty] private string ipaqCaminataDias = "0";
    [ObservableProperty] private string ipaqCaminataMinutos = "0";

    // --- SISCO: PLACEHOLDER, ver TODO abajo ---
    [ObservableProperty] private string puntajeSisco = "0";

    [ObservableProperty] private string mensajeError = string.Empty;

    [ObservableProperty] private bool enviado;

    public EncuestaBasalViewModel(IEncuestaBasalService encuestaBasalService, IAuthService authService)
    {
        this.encuestaBasalService = encuestaBasalService;
        this.authService = authService;
        Title = "Encuesta basal";
        PreguntasPhq9 = new ObservableCollection<PreguntaPhq9Item>(
            TextosPhq9.Select((texto, i) => new PreguntaPhq9Item(i + 1, texto)));
    }

    [RelayCommand]
    private async Task EnviarAsync()
    {
        if (IsBusy) return;
        MensajeError = string.Empty;

        if (PreguntasPhq9.Any(p => p.Respuesta < 0))
        {
            MensajeError = "Responde las 9 preguntas del PHQ-9 (todas son obligatorias).";
            return;
        }

        if (!TryParseIpaq(out var puntajeIpaq, out var errorIpaq))
        {
            MensajeError = errorIpaq;
            return;
        }

        if (!int.TryParse(PuntajeSisco, out var puntajeSiscoValor) || puntajeSiscoValor < 0)
        {
            MensajeError = "El puntaje SISCO debe ser un número.";
            return;
        }

        try
        {
            IsBusy = true;

            var respuesta = await encuestaBasalService.RegistrarAsync(new EncuestaBasalRequest
            {
                Fase = FaseSeleccionada == "Final" ? FaseEncuesta.Final : FaseEncuesta.Basal,
                RespuestasPhq9 = [.. PreguntasPhq9.Select(p => p.Respuesta)],
                PuntajeSISCO = puntajeSiscoValor,
                PuntajeIPAQ = puntajeIpaq,
            });

            Enviado = true;

            if (respuesta.RequiereAtencionInmediata)
            {
                // TODO CRÍTICO — antes de usar con estudiantes reales:
                // reemplazar este texto y el contacto por el que apruebe tu
                // asesor/comité de ética. No lanzar el piloto con este
                // mensaje de ejemplo.
                await Shell.Current.CurrentPage.DisplayAlertAsync(
                    "Queremos que sepas que no estás solo(a)",
                    "Notamos una respuesta que sugiere que podrías estar pasando por un momento difícil. " +
                    "Por favor comunícate con: [TODO: completar con la línea/contacto de tu universidad o " +
                    "servicio de salud mental aprobado por tu comité de ética].",
                    "Entendido");
            }
        }
        catch (SesionExpiradaException)
        {
            await VolverAlLoginAsync();
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        catch (Exception)
        {
            MensajeError = "No se pudo conectar con el servidor. Revisa tu conexión y la URL de la API.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private static async Task VolverAsync() => await Shell.Current.GoToAsync("..");

    private async Task VolverAlLoginAsync()
    {
        await authService.CerrarSesionAsync();
        await Shell.Current.DisplayAlertAsync("Sesión expirada", "Tu sesión expiró. Vuelve a iniciar sesión.", "OK");
        await Shell.Current.GoToAsync("//LoginPage");
    }

    /// <summary>
    /// Fórmula oficial de puntuación del IPAQ corto: MET-minutos/semana =
    /// (8.0 × min vigorosos × días vigorosos) + (4.0 × min moderados × días
    /// moderados) + (3.3 × min caminando × días caminando).
    /// </summary>
    private bool TryParseIpaq(out int puntajeIpaq, out string error)
    {
        puntajeIpaq = 0;
        error = string.Empty;

        if (!TryParseDiasMinutos(IpaqVigorosoDias, IpaqVigorosoMinutos, out var vigDias, out var vigMin) ||
            !TryParseDiasMinutos(IpaqModeradoDias, IpaqModeradoMinutos, out var modDias, out var modMin) ||
            !TryParseDiasMinutos(IpaqCaminataDias, IpaqCaminataMinutos, out var camDias, out var camMin))
        {
            error = "En el IPAQ, los días deben ser de 0 a 7 y los minutos un número válido.";
            return false;
        }

        puntajeIpaq = (int)Math.Round((8.0 * vigMin * vigDias) + (4.0 * modMin * modDias) + (3.3 * camMin * camDias));
        return true;
    }

    private static bool TryParseDiasMinutos(string diasTexto, string minutosTexto, out int dias, out int minutos)
    {
        dias = 0;
        minutos = 0;
        return int.TryParse(diasTexto, out dias) && dias is >= 0 and <= 7 &&
               int.TryParse(minutosTexto, out minutos) && minutos >= 0;
    }
}
