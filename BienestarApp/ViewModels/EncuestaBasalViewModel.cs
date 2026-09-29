using System.Collections.ObjectModel;
using BienestarApp.Models;
using BienestarApp.Services;
using BienestarApp.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BienestarApp.ViewModels;

/// <summary>
/// Encuesta basal (fase Basal o Final del diseño pre-post): PHQ-9 completo
/// (con el protocolo del ítem 9), IPAQ corto, y SISCO SV-21 — los tres
/// instrumentos se reproducen tal cual sus fuentes originales, sin
/// parafrasear ni reordenar ítems (afectaría su validez psicométrica).
///
/// Se presenta como asistente de 4 pasos (uno por instrumento + revisión)
/// en vez de un formulario largo de una sola pantalla: son 37 preguntas en
/// total, y de un solo tirón resulta abrumador para responder bien.
/// </summary>
public partial class EncuestaBasalViewModel : BaseViewModel
{
    private readonly IEncuestaBasalService encuestaBasalService;
    private readonly IAuthService authService;
    private readonly ISincronizacionService sincronizacion;

    private const int TotalPasos = 4;
    private FaseEncuesta? faseActual;

    // ---------- PHQ-9 (Patient Health Questionnaire-9) ----------
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

    // ---------- SISCO SV-21 (Inventario SISCO, Barraza, 2018) ----------
    private static readonly string[] TextosSiscoEstresores =
    [
        "La sobrecarga de tareas y trabajos escolares que tengo que realizar todos los días",
        "La personalidad y el carácter de los/as profesores/as que me imparten clases",
        "La forma de evaluación de mis profesores/as (a través de ensayos, trabajos de investigación, búsquedas en Internet, etc.)",
        "El nivel de exigencia de mis profesores/as",
        "El tipo de trabajo que me piden los profesores (consulta de temas, fichas de trabajo, ensayos, mapas conceptuales, etc.)",
        "Tener tiempo limitado para hacer el trabajo que me encargan los/as profesores/as",
        "La poca claridad que tengo sobre lo que quieren los/as profesores/as",
    ];
    private static readonly string[] TextosSiscoSintomas =
    [
        "Fatiga crónica (cansancio permanente)",
        "Sentimientos de depresión y tristeza (decaído)",
        "Ansiedad, angustia o desesperación",
        "Problemas de concentración",
        "Sentimiento de agresividad o aumento de irritabilidad",
        "Conflictos o tendencia a polemizar o discutir",
        "Desgano para realizar las labores escolares",
    ];
    private static readonly string[] TextosSiscoEstrategias =
    [
        "Concentrarse en resolver la situación que me preocupa",
        "Establecer soluciones concretas para resolver la situación que me preocupa",
        "Analizar lo positivo y negativo de las soluciones pensadas para solucionar la situación que me preocupa",
        "Mantener el control sobre mis emociones para que no me afecte lo que me estresa",
        "Recordar situaciones similares ocurridas anteriormente y pensar en cómo las solucioné",
        "Elaboración de un plan para enfrentar lo que me estresa y ejecución de sus tareas",
        "Fijarse o tratar de obtener lo positivo de la situación que preocupa",
    ];

    public ObservableCollection<PreguntaPhq9Item> PreguntasPhq9 { get; }
    public ObservableCollection<PreguntaSiscoItem> PreguntasEstresores { get; }
    public ObservableCollection<PreguntaSiscoItem> PreguntasSintomas { get; }
    public ObservableCollection<PreguntaSiscoItem> PreguntasEstrategias { get; }

    public List<string> OpcionesSiNo { get; } = ["Sí", "No"];

    // ---------- Estado del asistente (wizard) ----------
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EnPasoPhq9))]
    [NotifyPropertyChangedFor(nameof(EnPasoIpaq))]
    [NotifyPropertyChangedFor(nameof(EnPasoSisco))]
    [NotifyPropertyChangedFor(nameof(EnPasoRevision))]
    [NotifyPropertyChangedFor(nameof(MostrarAnterior))]
    [NotifyPropertyChangedFor(nameof(TextoPaso))]
    [NotifyPropertyChangedFor(nameof(IconoPaso))]
    [NotifyPropertyChangedFor(nameof(TituloPaso))]
    [NotifyPropertyChangedFor(nameof(Progreso))]
    private int paso;

    public bool EnPasoPhq9 => Paso == 0;
    public bool EnPasoIpaq => Paso == 1;
    public bool EnPasoSisco => Paso == 2;
    public bool EnPasoRevision => Paso == 3;
    public bool MostrarAnterior => Paso > 0;
    public string TextoPaso => $"Paso {Paso + 1} de {TotalPasos}";
    public double Progreso => (Paso + 1) / (double)TotalPasos;

    // Ícono + título grandes por paso — el toque "vivo" que pediste, sin
    // depender de imágenes externas: son emoji (se ven en cualquier
    // celular, sin descargar nada) y la Vista los anima con un pequeño
    // "rebote" cada vez que cambias de paso (ver EncuestaBasalPage.xaml.cs).
    public string IconoPaso => Paso switch
    {
        0 => "🧠",
        1 => "🏃",
        2 => "📚",
        _ => "✅",
    };
    public string TituloPaso => Paso switch
    {
        0 => "PHQ-9 · Estado de ánimo",
        1 => "IPAQ · Actividad física",
        2 => "SISCO · Estrés académico",
        _ => "Revisión y envío",
    };

    // Ítem 1 (filtro) del SISCO.
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MostrarPreguntasSisco))]
    private string respuestaFiltroSisco = string.Empty;

    public bool MostrarPreguntasSisco => RespuestaFiltroSisco == "Sí";

    // --- IPAQ corto ---
    [ObservableProperty] private string ipaqVigorosoDias = "0";
    [ObservableProperty] private string ipaqVigorosoMinutos = "0";
    [ObservableProperty] private string ipaqModeradoDias = "0";
    [ObservableProperty] private string ipaqModeradoMinutos = "0";
    [ObservableProperty] private string ipaqCaminataDias = "0";
    [ObservableProperty] private string ipaqCaminataMinutos = "0";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MostrarAsistente))]
    private string mensajeError = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MostrarAsistente))]
    private bool enviado;

    [ObservableProperty] private string resumenPrevio = string.Empty;

    // --- Estado de acceso (gating por fase) ---
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MostrarAsistente))]
    private bool cargandoEstado = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MostrarAsistente))]
    private bool accesoBloqueado;

    [ObservableProperty] private string mensajeBloqueo = string.Empty;
    [ObservableProperty] private string faseActualTexto = string.Empty;

    /// <summary>Solo se ve el asistente cuando ya se sabe la fase, el acceso no está bloqueado, y no se envió todavía.</summary>
    public bool MostrarAsistente => !CargandoEstado && !AccesoBloqueado && !Enviado;

    public EncuestaBasalViewModel(IEncuestaBasalService encuestaBasalService, IAuthService authService, ISincronizacionService sincronizacion)
    {
        this.encuestaBasalService = encuestaBasalService;
        this.authService = authService;
        this.sincronizacion = sincronizacion;
        Title = "Encuesta basal";

        PreguntasPhq9 = new ObservableCollection<PreguntaPhq9Item>(
            TextosPhq9.Select((texto, i) => new PreguntaPhq9Item(i + 1, texto)));
        PreguntasEstresores = new ObservableCollection<PreguntaSiscoItem>(TextosSiscoEstresores.Select(t => new PreguntaSiscoItem(t)));
        PreguntasSintomas = new ObservableCollection<PreguntaSiscoItem>(TextosSiscoSintomas.Select(t => new PreguntaSiscoItem(t)));
        PreguntasEstrategias = new ObservableCollection<PreguntaSiscoItem>(TextosSiscoEstrategias.Select(t => new PreguntaSiscoItem(t)));
    }

    /// <summary>
    /// Decide qué fase toca (Basal si no existe, si no Final) y bloquea el
    /// acceso si ya se completaron las dos o si la final todavía no se
    /// habilita (la fecha la decide la API: "Piloto:FechaHabilitaEncuestaFinal").
    /// Se llama al abrir la pantalla — nunca se confía solo en la validación
    /// del servidor al enviar, para no dejar que alguien llene 37 preguntas
    /// y recién ahí se entere de que no podía. Es una sola consulta y esta
    /// pantalla se abre muy pocas veces en todo el piloto.
    /// </summary>
    public async Task InicializarAsync()
    {
        CargandoEstado = true;
        AccesoBloqueado = false;
        MensajeError = string.Empty;
        Paso = 0;

        try
        {
            var datos = await sincronizacion.ActualizarEstadoEncuestasAsync();
            var hoy = DateOnly.FromDateTime(DateTime.Now);

            if (datos.BasalCompletada && datos.FinalCompletada)
            {
                AccesoBloqueado = true;
                MensajeBloqueo = "Ya completaste las dos encuestas (inicial y de cierre). ¡Gracias por participar!";
                faseActual = null;
            }
            else if (datos.BasalCompletada && !datos.FinalDisponible(hoy))
            {
                AccesoBloqueado = true;
                MensajeBloqueo = datos.FechaHabilitaFinal is { } fecha
                    ? $"Ya completaste la encuesta inicial. La encuesta de cierre se habilita el {fecha:dd/MM/yyyy}; te avisaremos por el canal del piloto."
                    : "Ya completaste la encuesta inicial. La encuesta de cierre se habilitará al final del piloto.";
                faseActual = null;
            }
            else if (datos.BasalCompletada)
            {
                faseActual = FaseEncuesta.Final;
                FaseActualTexto = "Fase: Final (cierre del piloto)";
            }
            else
            {
                faseActual = FaseEncuesta.Basal;
                FaseActualTexto = "Fase: Basal (inicial)";
            }
        }
        catch (SesionExpiradaException)
        {
            await VolverAlLoginAsync();
        }
        catch (Exception)
        {
            MensajeError = "No se pudo comprobar el estado de tu encuesta. Revisa tu conexión y vuelve a intentar.";
            AccesoBloqueado = true; // más seguro no dejar llenar 37 preguntas sin saber si ya las hizo.
        }
        finally
        {
            CargandoEstado = false;
        }
    }

    [RelayCommand]
    private void Siguiente()
    {
        MensajeError = string.Empty;

        if (Paso == 0 && PreguntasPhq9.Any(p => p.Respuesta < 0))
        {
            MensajeError = "Responde las 9 preguntas del PHQ-9 (todas son obligatorias).";
            return;
        }
        if (Paso == 1 && !TryParseIpaq(out _, out var errorIpaq))
        {
            MensajeError = errorIpaq;
            return;
        }
        if (Paso == 2 && !TryCalcularPuntajeSisco(out _, out var errorSisco))
        {
            MensajeError = errorSisco;
            return;
        }

        if (Paso == 2) ArmarResumenPrevio();
        Paso++;
    }

    [RelayCommand]
    private void Anterior()
    {
        MensajeError = string.Empty;
        if (Paso > 0) Paso--;
    }

    [RelayCommand]
    private async Task EnviarAsync()
    {
        if (IsBusy || faseActual is null) return;
        MensajeError = string.Empty;

        try
        {
            IsBusy = true;
            TryParseIpaq(out var puntajeIpaq, out _);
            TryCalcularPuntajeSisco(out var puntajeSisco, out _);

            var respuesta = await encuestaBasalService.RegistrarAsync(new EncuestaBasalRequest
            {
                Fase = faseActual.Value,
                RespuestasPhq9 = [.. PreguntasPhq9.Select(p => p.Respuesta)],
                PuntajeSISCO = puntajeSisco,
                PuntajeIPAQ = puntajeIpaq,
            });

            Enviado = true;
            sincronizacion.RegistrarEncuesta(faseActual.Value);

            if (respuesta.RequiereAtencionInmediata)
            {
                await Shell.Current.CurrentPage.DisplayAlertAsync(
                    "Queremos que sepas que no estás solo(a)",
                    "Notamos una respuesta que sugiere que podrías estar pasando por un momento difícil. " +
                    "Por favor comunícate con la Línea 113, opción Salud Mental (Recibe Ayuda) del Ministerio " +
                    "de Salud, disponible las 24 horas, o acude al servicio de bienestar/salud de tu universidad.",
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
            MensajeError = "No se pudo enviar la encuesta. Revisa tu conexión con el servidor.";
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

    private void ArmarResumenPrevio()
    {
        TryParseIpaq(out var puntajeIpaq, out _);
        TryCalcularPuntajeSisco(out var puntajeSisco, out _);
        var puntajePhq9 = PreguntasPhq9.Sum(p => p.Respuesta);

        ResumenPrevio =
            $"PHQ-9: {puntajePhq9}/27\n" +
            $"IPAQ: {puntajeIpaq} MET-min/semana\n" +
            $"SISCO: {puntajeSisco}/100" + (RespuestaFiltroSisco == "No" ? " (sin estrés académico reportado)" : "");
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

    /// <summary>
    /// "Clave de corrección" oficial del SISCO SV-21: media de los 21 ítems
    /// (estresores + síntomas + estrategias, codificados 0-5) transformada
    /// a porcentaje (media × 20). Si el filtro fue "No", el estrés académico
    /// no está presente y el puntaje es 0, sin pedir los 21 ítems.
    /// </summary>
    private bool TryCalcularPuntajeSisco(out int puntajeSisco, out string error)
    {
        puntajeSisco = 0;
        error = string.Empty;

        if (string.IsNullOrEmpty(RespuestaFiltroSisco))
        {
            error = "Responde si has tenido momentos de preocupación o nerviosismo (estrés) este semestre.";
            return false;
        }

        if (RespuestaFiltroSisco == "No")
        {
            puntajeSisco = 0;
            return true;
        }

        var todasLasPreguntas = PreguntasEstresores.Concat(PreguntasSintomas).Concat(PreguntasEstrategias).ToList();
        if (todasLasPreguntas.Any(p => p.Respuesta < 0))
        {
            error = "Responde las 21 preguntas del SISCO (estresores, síntomas y estrategias de afrontamiento).";
            return false;
        }

        var media = todasLasPreguntas.Average(p => p.Respuesta);
        puntajeSisco = Math.Clamp((int)Math.Round(media * 20), 0, 100);
        return true;
    }
}
