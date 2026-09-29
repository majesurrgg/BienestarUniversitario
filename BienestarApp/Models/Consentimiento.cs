namespace BienestarApp.Models;

/// <summary>
/// Texto del consentimiento informado que se muestra al registrarse. La API
/// guarda qué versión aceptó cada estudiante y cuándo (DimUsuario), así que
/// si el texto cambia tras la revisión del asesor/comité hay que subir
/// <see cref="Version"/>.
///
/// PROVISIONAL: reemplazar por el texto aprobado por el asesor antes del
/// piloto, y completar el dato de contacto.
/// </summary>
public static class Consentimiento
{
    public const string Version = "v0.1-provisional";

    public const string Investigadora = "María Jesús Reyes";
    public const string Universidad = "Universidad Tecnológica del Perú";

    // TODO: completar antes de generar el APK del piloto (correo o WhatsApp).
    public const string Contacto = "[COMPLETAR: correo o WhatsApp de contacto]";

    public const string Texto =
        "CONSENTIMIENTO INFORMADO\n" +
        "Piloto de tesis «Bienestar Universitario»\n\n" +

        "Investigadora: " + Investigadora + ", " + Universidad + ".\n\n" +

        "¿De qué se trata?\n" +
        "Este estudio busca mejorar el monitoreo del bienestar físico y mental de estudiantes " +
        "universitarios de Arequipa mediante una aplicación móvil.\n\n" +

        "¿Qué tendrías que hacer?\n" +
        "• Al inicio: responder una encuesta (aprox. 10 minutos) sobre estado de ánimo (PHQ-9), " +
        "estrés académico (SISCO) y actividad física (IPAQ).\n" +
        "• Durante 4 semanas: registrar cada día, en 1 minuto, tu nivel de estrés, sueño, ánimo y " +
        "actividad física.\n" +
        "• Al final: repetir la encuesta inicial y evaluar qué tan fácil fue usar la app (SUS).\n\n" +

        "¿Qué datos se recogen?\n" +
        "Tu nombre, código universitario, carrera y correo, y tus respuestas a las encuestas y registros " +
        "diarios. Algunas de estas respuestas son datos de salud, considerados datos sensibles por la " +
        "Ley N.° 29733 de Protección de Datos Personales.\n\n" +

        "Confidencialidad\n" +
        "Solo la investigadora tiene acceso a tus datos. Los resultados se presentan de forma agregada, " +
        "sin nombres, y se usan únicamente con fines de investigación. Los datos se almacenan cifrados en " +
        "servidores de Microsoft Azure ubicados en Estados Unidos.\n\n" +

        "Si una respuesta indica que podrías estar en riesgo\n" +
        "La encuesta incluye una pregunta sobre pensamientos de hacerse daño. Si tu respuesta sugiere que " +
        "podrías estar pasando por un momento difícil, la app te mostrará de inmediato información de " +
        "ayuda (Línea 113, opción Salud Mental, del Ministerio de Salud, disponible las 24 horas) y quedará " +
        "un registro para que la investigadora pueda seguir el protocolo del estudio. Esta app no es un " +
        "servicio de diagnóstico ni reemplaza la atención profesional.\n\n" +

        "Participación voluntaria\n" +
        "Participar es voluntario. Puedes retirarte en cualquier momento, sin dar explicaciones y sin " +
        "ninguna consecuencia, y pedir que se eliminen tus datos escribiendo a la investigadora.\n\n" +

        "Incentivos\n" +
        "Quienes participen entran en sorteos por Yape, según las bases publicadas en el canal del piloto. " +
        "Participar o no en los sorteos no afecta tu participación en el estudio.\n\n" +

        "Contacto\n" +
        Investigadora + ": " + Contacto + "\n\n" +

        "Al marcar «Acepto participar», declaras ser mayor de 18 años, haber leído esta información y " +
        "aceptar participar voluntariamente en el estudio.";
}
