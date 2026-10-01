namespace BienestarApp.Services;

/// <summary>
/// Una frase breve de bienestar distinta cada día en la pantalla principal.
/// Lista fija dentro de la app: no consulta la API ni la base de datos, así
/// que no consume el cupo de Azure. Frases propias (no citas de autores),
/// en tono cercano y sin consejos clínicos.
/// </summary>
public static class FrasesDelDia
{
    private static readonly string[] Frases =
    [
        "Un paso pequeño también es avanzar.",
        "Descansar también es parte de rendir bien.",
        "No tienes que poder con todo hoy.",
        "Tu valor no depende de una nota.",
        "Respira hondo: este momento también pasa.",
        "Hacer una pausa no es perder el tiempo.",
        "Pedir ayuda es una forma de cuidarte.",
        "Hoy basta con hacer lo que puedas.",
        "Moverte un poco cambia cómo te sientes.",
        "Dormir bien es estudiar mejor mañana.",
        "Celebra lo que sí lograste hoy.",
        "Está bien no estar bien todos los días.",
        "Tu ritmo es válido, aunque sea distinto al de otros.",
        "Un vaso de agua y cinco minutos de aire también cuentan.",
        "Lo que sientes importa.",
        "Ser constante es más poderoso que ser perfecto.",
        "Habla contigo como hablarías con un buen amigo.",
        "Cada día es una nueva oportunidad para empezar.",
        "Escuchar a tu cuerpo es una forma de sabiduría.",
        "No compares tu capítulo 1 con el capítulo 20 de otro.",
        "Un buen día empieza por una pequeña decisión.",
        "Tu esfuerzo de hoy es la base de tu mañana.",
        "Desconectarte un rato también recarga.",
        "La calma también se entrena.",
        "Reírte un poco es un buen descanso.",
        "Mereces el mismo cuidado que das a los demás.",
        "Organizar tu día es regalarte tranquilidad.",
        "Vas mejor de lo que crees.",
        "Un minuto para ti también es importante.",
        "Gracias por tomarte este momento para ti.",
    ];

    /// <summary>La frase de esa fecha: la misma durante todo el día, distinta al día siguiente.</summary>
    public static string Para(DateOnly fecha) => Frases[fecha.DayNumber % Frases.Length];
}
