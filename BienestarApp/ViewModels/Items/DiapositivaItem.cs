namespace BienestarApp.ViewModels.Items;

/// <summary>Una diapositiva de la bienvenida (primera vez que se abre la app).</summary>
public class DiapositivaItem
{
    public required string Emoji { get; init; }
    public required string Titulo { get; init; }
    public required string Texto { get; init; }
}
